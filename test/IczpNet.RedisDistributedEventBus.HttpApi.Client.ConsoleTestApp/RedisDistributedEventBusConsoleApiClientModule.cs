using Volo.Abp.Autofac;
using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace IczpNet.RedisDistributedEventBus;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(RedisDistributedEventBusHttpApiClientModule),
    typeof(AbpHttpClientIdentityModelModule)
    )]
public class RedisDistributedEventBusConsoleApiClientModule : AbpModule
{

}
