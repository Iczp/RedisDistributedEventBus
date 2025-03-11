using IczpNet.RedisDistributedEventBus.Localization;
using Volo.Abp.Application.Services;

namespace IczpNet.RedisDistributedEventBus;

public abstract class RedisDistributedEventBusAppService : ApplicationService
{
    protected RedisDistributedEventBusAppService()
    {
        LocalizationResource = typeof(RedisDistributedEventBusResource);
        ObjectMapperContext = typeof(RedisDistributedEventBusApplicationModule);
    }
}
