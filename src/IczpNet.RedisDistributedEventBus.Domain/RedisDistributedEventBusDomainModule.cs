using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace IczpNet.RedisDistributedEventBus;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(RedisDistributedEventBusDomainSharedModule)
)]

[DependsOn(typeof(RedisDistributedEventBusModule))]
public class RedisDistributedEventBusDomainModule : AbpModule
{


}
