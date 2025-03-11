using IczpNet.RedisDistributedEventBus.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace IczpNet.RedisDistributedEventBus;

public abstract class RedisDistributedEventBusController : AbpControllerBase
{
    protected RedisDistributedEventBusController()
    {
        LocalizationResource = typeof(RedisDistributedEventBusResource);
    }
}
