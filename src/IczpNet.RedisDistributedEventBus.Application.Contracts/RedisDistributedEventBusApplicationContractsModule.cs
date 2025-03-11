using Volo.Abp.Application;
using Volo.Abp.Modularity;
using Volo.Abp.Authorization;

namespace IczpNet.RedisDistributedEventBus;

[DependsOn(
    typeof(RedisDistributedEventBusDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationModule)
    )]
public class RedisDistributedEventBusApplicationContractsModule : AbpModule
{

}
