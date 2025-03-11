using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Guids;
using Volo.Abp.Json;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Threading;
using Volo.Abp.Timing;
using Volo.Abp.Tracing;
using Volo.Abp.Uow;

namespace IczpNet.RedisDistributedEventBus;

[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(IDistributedEventBus), typeof(RedisDistributedEventBus), typeof(IRedisDistributedEventBus))]
public class RedisDistributedEventBus(
    IOptions<RedisDistributedEventBusOptions> options,
    IServiceScopeFactory serviceScopeFactory,
    IConnectionMultiplexer redis,
    IJsonSerializer jsonSerializer,
    ILogger<RedisDistributedEventBus> logger,
    ICurrentTenant currentTenant,
    IUnitOfWorkManager unitOfWorkManager,
    IOptions<AbpDistributedEventBusOptions> abpDistributedEventBusOptions,
    IGuidGenerator guidGenerator,
    IClock clock,
    IEventHandlerInvoker eventHandlerInvoker,
    ILocalEventBus localEventBus,
    ICorrelationIdProvider correlationIdProvider) : DistributedEventBusBase(
    serviceScopeFactory,
    currentTenant,
    unitOfWorkManager,
    abpDistributedEventBusOptions,
    guidGenerator,
    clock,
    eventHandlerInvoker,
    localEventBus,
    correlationIdProvider), IRedisDistributedEventBus, ISingletonDependency
{
    protected RedisDistributedEventBusOptions Config { get; } = options.Value;
    protected IConnectionMultiplexer Redis { get; } = redis;
    protected ISubscriber? Subscriber { get; }
    protected ILogger<RedisDistributedEventBus> Logger { get; } = logger ?? NullLogger<RedisDistributedEventBus>.Instance;
    protected IJsonSerializer JsonSerializer { get; } = jsonSerializer;
    protected virtual string ChannelPrefix { get; } = $"{nameof(RedisDistributedEventBus)}:";
    protected ConcurrentDictionary<Type, List<IEventHandlerFactory>> HandlerFactories { get; } = new ConcurrentDictionary<Type, List<IEventHandlerFactory>>();
    protected ConcurrentDictionary<string, Type> EventTypes { get; } = new ConcurrentDictionary<string, Type>();

    public virtual void Initialize()
    {
        Logger.LogWarning($"{nameof(RedisDistributedEventBus)} Initialize, totalCount:{AbpDistributedEventBusOptions.Handlers.Count}.");

        SubscribeHandlers(AbpDistributedEventBusOptions.Handlers);

        foreach (var type in AbpDistributedEventBusOptions.Handlers)
        {
            Logger.LogWarning($"{type.FullName} Subscribed.");
        }
    }

    private async Task ProcessEventAsync(string channel, string message)
    {
        var eventName = GetEventName(channel);

        var eventType = EventTypes.GetOrDefault(eventName);

        if (eventType == null)
        {
            return;
        }

        var eventData = DeserializeEventData(message, eventType);

        var correlationId = GetCorrelationIdFromHeader(message);

        if (await AddToInboxAsync(GuidGenerator.Create().ToString(), eventName, eventType, eventData, correlationId))
        {
            return;
        }

        using (CorrelationIdProvider.Change(correlationId))
        {
            await TriggerHandlersDirectAsync(eventType, eventData);
        }
    }
    protected virtual string? GetCorrelationIdFromHeader(string message)
    {
        // todo: 这里根据你的消息格式 获取 correlationId
        return null;
    }
    protected virtual object DeserializeEventData(object message, Type eventType)
    {
        var method = typeof(RedisDistributedEventBus).GetMethod(nameof(DeserializeInternal), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (method == null)
        {
            throw new Exception($"{nameof(RedisDistributedEventBus)}.${nameof(DeserializeInternal)} not found!");
        }

        var genericMethod = method.MakeGenericMethod(eventType);

        var result = genericMethod.Invoke(this, [message]);

        if (result == null)
        {
            throw new Exception($"Deserialization of message to {eventType.FullName} returned null.");
        }
        return result;
    }

    private T DeserializeInternal<T>(string message)
    {
        return JsonSerializer.Deserialize<T>(message);
    }

    protected virtual string GetEventName(string channel)
    {
        //return channel.Replace(ChannelPrefix, "");
        return channel.Substring(ChannelPrefix.Length);
    }

    protected virtual string GetChannelName(Type eventType)
    {
        return GetChannelName(EventNameAttribute.GetNameOrDefault(eventType));
    }

    protected virtual string GetChannelName(string eventName)
    {
        return $"{ChannelPrefix}{eventName}";
    }

    public override IDisposable Subscribe(Type eventType, IEventHandlerFactory factory)
    {
        var handlerFactories = GetOrCreateHandlerFactories(eventType);

        if (factory.IsInFactories(handlerFactories))
        {
            return NullDisposable.Instance;
        }

        handlerFactories.Add(factory);

        if (handlerFactories.Count == 1) //TODO: Multi-threading!
        {

            var channelName = GetChannelName(eventType);

            var channel = new RedisChannel(channelName, RedisChannel.PatternMode.Literal);

            var subscriber = Redis.GetSubscriber();

            subscriber.SubscribeAsync(channel, (channel, message) =>
            {
                AsyncHelper.RunSync(async () => await ProcessEventAsync(channel!, message!));
            });

            Logger.LogInformation($"Subscribed to Redis channel. Event Type: {eventType.FullName}, Channel: {channelName}");
        }

        return new EventHandlerFactoryUnregistrar(this, eventType, factory);
    }
    public override void Unsubscribe<TEvent>(Func<TEvent, Task> action)
    {
        Check.NotNull(action, nameof(action));

        GetOrCreateHandlerFactories(typeof(TEvent))
            .Locking(factories =>
            {
                factories.RemoveAll(
                    factory =>
                    {
                        if (factory is not SingleInstanceHandlerFactory singleInstanceFactory)
                        {
                            return false;
                        }

                        if (singleInstanceFactory.HandlerInstance is not ActionEventHandler<TEvent> actionHandler)
                        {
                            return false;
                        }

                        return actionHandler.Action == action;
                    });
            });
    }

    public override void Unsubscribe(Type eventType, IEventHandler handler)
    {
        GetOrCreateHandlerFactories(eventType)
            .Locking(factories =>
            {
                factories.RemoveAll(
                    factory =>
                        factory is SingleInstanceHandlerFactory &&
                        (factory as SingleInstanceHandlerFactory)!.HandlerInstance == handler
                );
            });
    }

    public override void Unsubscribe(Type eventType, IEventHandlerFactory factory)
    {
        GetOrCreateHandlerFactories(eventType).Locking(factories => factories.Remove(factory));
    }

    public override void UnsubscribeAll(Type eventType)
    {
        GetOrCreateHandlerFactories(eventType).Locking(factories => factories.Clear());
    }

    protected override void AddToUnitOfWork(IUnitOfWork unitOfWork, UnitOfWorkEventRecord eventRecord)
    {
        unitOfWork.AddOrReplaceDistributedEvent(eventRecord);
        Logger.LogDebug($"Added event to unit of work. EventType={eventRecord.EventType}, EventOrder={eventRecord.EventOrder}, EventData={eventRecord.EventData}");
    }
    public override async Task PublishFromOutboxAsync(OutgoingEventInfo outgoingEvent, OutboxConfig outboxConfig)
    {
        using (CorrelationIdProvider.Change(outgoingEvent.GetCorrelationId()))
        {
            await TriggerDistributedEventSentAsync(new DistributedEventSent()
            {
                Source = DistributedEventSource.Outbox,
                EventName = outgoingEvent.EventName,
                EventData = outgoingEvent.EventData
            });
        }
        await PublishAsync(outgoingEvent.EventName, outgoingEvent.EventData, eventId: outgoingEvent.Id, correlationId: outgoingEvent.GetCorrelationId());
    }

    public override async Task PublishManyFromOutboxAsync(IEnumerable<OutgoingEventInfo> outgoingEvents, OutboxConfig outboxConfig)
    {
        foreach (var outgoingEvent in outgoingEvents)
        {
            await PublishFromOutboxAsync(outgoingEvent, outboxConfig);
        }
    }
    public override async Task ProcessFromInboxAsync(IncomingEventInfo incomingEvent, InboxConfig inboxConfig)
    {
        var eventType = EventTypes.GetOrDefault(incomingEvent.EventName);

        if (eventType == null)
        {
            return;
        }

        var eventData = DeserializeEventData(incomingEvent.EventData, eventType);

        var exceptions = new List<Exception>();

        using (CorrelationIdProvider.Change(incomingEvent.GetCorrelationId()))
        {
            await TriggerHandlersFromInboxAsync(eventType, eventData, exceptions, inboxConfig);
        }

        if (exceptions.Any())
        {
            ThrowOriginalExceptions(eventType, exceptions);
        }
    }

    protected override byte[] Serialize(object eventData)
    {
        var jsonString = JsonSerializer.Serialize(eventData);

        return Encoding.UTF8.GetBytes(jsonString);
    }

    protected async override Task PublishToEventBusAsync(Type eventType, object eventData)
    {
        await PublishAsync(eventType, eventData, correlationId: CorrelationIdProvider.Get());
    }
    public virtual Task PublishAsync(
        Type eventType,
        object eventData,
        Dictionary<string, object>? headersArguments = null,
        Guid? eventId = null,
        string? correlationId = null)
    {
        var eventName = EventNameAttribute.GetNameOrDefault(eventType);
        var body = Serialize(eventData);
        return PublishAsync(eventName, body, headersArguments, eventId, correlationId);
    }

    protected virtual Task PublishAsync(
         string eventName,
         byte[] body,
         Dictionary<string, object>? headersArguments = null,
         Guid? eventId = null,
         string? correlationId = null)
    {
        return PublishToRedisAsync(eventName, body, headersArguments, eventId, correlationId);
    }

    protected virtual async Task PublishToRedisAsync(
        string eventName,
        byte[] body,
        Dictionary<string, object>? headersArguments = null,
        Guid? eventId = null,
        string? correlationId = null)
    {
        var channel = new RedisChannel(GetChannelName(eventName), RedisChannel.PatternMode.Literal);

        var subscriber = Redis.GetSubscriber();
        try
        {
            await subscriber.PublishAsync(channel, body);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Error publishing event to Redis. Event Name: {eventName}");
            throw;
        }
        Logger.LogDebug($"Published event to Redis. Event Name: {eventName}, Channel: {channel}, eventId: {eventId}, correlationId: {correlationId}, headersArguments:{headersArguments}");
    }

    protected override Task OnAddToOutboxAsync(string eventName, Type eventType, object eventData)
    {
        EventTypes.GetOrAdd(eventName, eventType);
        return base.OnAddToOutboxAsync(eventName, eventType, eventData);
    }

    protected virtual List<IEventHandlerFactory> GetOrCreateHandlerFactories(Type eventType)
    {
        return HandlerFactories.GetOrAdd(eventType, type =>
        {
            var eventName = EventNameAttribute.GetNameOrDefault(type);
            EventTypes.GetOrAdd(eventName, eventType);
            return [];
        });
    }

    protected override IEnumerable<EventTypeWithEventHandlerFactories> GetHandlerFactories(Type eventType)
    {
        var handlerFactoryList = new List<EventTypeWithEventHandlerFactories>();

        foreach (var handlerFactory in
                 HandlerFactories.Where(hf => ShouldTriggerEventForHandler(eventType, hf.Key)))
        {
            handlerFactoryList.Add(
                new EventTypeWithEventHandlerFactories(handlerFactory.Key, handlerFactory.Value));
        }

        return [.. handlerFactoryList];
    }

    protected virtual bool ShouldTriggerEventForHandler(Type targetEventType, Type handlerEventType)
    {
        //Should trigger same type
        if (handlerEventType == targetEventType)
        {
            return true;
        }

        //Should trigger for inherited types
        if (handlerEventType.IsAssignableFrom(targetEventType))
        {
            return true;
        }

        return false;
    }
}