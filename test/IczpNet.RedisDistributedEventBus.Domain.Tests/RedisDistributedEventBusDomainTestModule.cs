using Volo.Abp.Modularity;

namespace IczpNet.RedisDistributedEventBus;

[DependsOn(
    typeof(RedisDistributedEventBusDomainModule),
    typeof(RedisDistributedEventBusTestBaseModule)
)]
public class RedisDistributedEventBusDomainTestModule : AbpModule
{

}
