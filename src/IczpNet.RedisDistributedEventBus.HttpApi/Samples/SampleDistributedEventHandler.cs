using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;

namespace IczpNet.RedisDistributedEventBus.Samples;

public class SampleDistributedEventHandler : DomainService, IDistributedEventHandler<SampleDto>, ITransientDependency
{
    public async Task HandleEventAsync(SampleDto eventData)
    {
        Logger.LogInformation($"Handled SampleDto: {eventData.Value}");

        await Task.CompletedTask;
    }
}
