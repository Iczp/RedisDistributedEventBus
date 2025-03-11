using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;

namespace IczpNet.RedisDistributedEventBus.Samples;

public class SampleDistributedEventHandler : DomainService, IDistributedEventHandler<SampleDto>, ITransientDependency
{
    public async Task HandleEventAsync(SampleDto eventData)
    {
        Logger.LogError($"Handled SampleDto: {eventData.Value}");

        await Task.CompletedTask;
    }
}
