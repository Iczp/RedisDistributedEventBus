using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.EventBus;
using Volo.Abp.Domain.Services;
using Microsoft.Extensions.Logging;
using Volo.Abp.Json;

namespace IczpNet.RedisDistributedEventBus.Handlers;

public class DistributedEventSentHandler(IJsonSerializer jsonSerializer) : DomainService, ILocalEventHandler<DistributedEventSent>, ITransientDependency
{
    public IJsonSerializer JsonSerializer { get; } = jsonSerializer;

    public async Task HandleEventAsync(DistributedEventSent eventData)
    {
        Logger.LogInformation($"发送分布式事件[{eventData.Source}]：EventName=${eventData.EventName},EventData={JsonSerializer.Serialize(eventData.EventData)}");

        await Task.CompletedTask;
    }
}