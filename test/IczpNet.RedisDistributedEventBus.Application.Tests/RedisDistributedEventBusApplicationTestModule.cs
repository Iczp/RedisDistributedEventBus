using Volo.Abp.Modularity;

namespace IczpNet.RedisDistributedEventBus;

[DependsOn(
    typeof(RedisDistributedEventBusApplicationModule),
    typeof(RedisDistributedEventBusDomainTestModule)
    )]
public class RedisDistributedEventBusApplicationTestModule : AbpModule
{

}
