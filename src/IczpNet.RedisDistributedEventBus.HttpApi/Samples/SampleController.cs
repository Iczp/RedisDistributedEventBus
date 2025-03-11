using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.EventBus.Distributed;

namespace IczpNet.RedisDistributedEventBus.Samples;

[Area(RedisDistributedEventBusRemoteServiceConsts.ModuleName)]
[RemoteService(Name = RedisDistributedEventBusRemoteServiceConsts.RemoteServiceName)]
[Route("api/RedisDistributedEventBus/sample")]
public class SampleController(
    ISampleAppService sampleAppService,
    IDistributedEventBus distributedEventBus
    ) : RedisDistributedEventBusController, ISampleAppService
{
    private readonly ISampleAppService _sampleAppService = sampleAppService;

    public IDistributedEventBus DistributedEventBus { get; } = distributedEventBus;

    [HttpGet]
    public async Task<SampleDto> GetAsync()
    {
        return await _sampleAppService.GetAsync();
    }

    [HttpPost]
    public async Task<SampleDto> PostAsync()
    {
        var ret = new SampleDto()
        {
            Value = DateTime.Now.Ticks
        };
        await DistributedEventBus.PublishAsync(ret);
        return ret;

    }

    [HttpGet]
    [Route("authorized")]
    [Authorize]
    public async Task<SampleDto> GetAuthorizedAsync()
    {
        return await _sampleAppService.GetAsync();
    }
}
