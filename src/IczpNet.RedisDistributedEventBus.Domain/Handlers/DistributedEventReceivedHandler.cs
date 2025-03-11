using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.EventBus;
using Volo.Abp.Domain.Services;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Volo.Abp.Json;

namespace IczpNet.RedisDistributedEventBus.Handlers;

public class DistributedEventReceivedHandler(IJsonSerializer jsonSerializer) : DomainService, ILocalEventHandler<DistributedEventReceived>, ITransientDependency
{
    public IJsonSerializer JsonSerializer { get; } = jsonSerializer;

    public async Task HandleEventAsync(DistributedEventReceived eventData)
    {
        // TODO: IMPLEMENT YOUR LOGIC...

        Logger.LogInformation($"收到分布式事件[{eventData.Source}]：EventName={eventData.EventName},EventData={JsonSerializer.Serialize(eventData.EventData)}");

        await Task.CompletedTask;
    }
}