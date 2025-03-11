using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace IczpNet.RedisDistributedEventBus;

[DependsOn(
    typeof(RedisDistributedEventBusApplicationContractsModule),
    typeof(AbpHttpClientModule))]
public class RedisDistributedEventBusHttpApiClientModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHttpClientProxies(
            typeof(RedisDistributedEventBusApplicationContractsModule).Assembly,
            RedisDistributedEventBusRemoteServiceConsts.RemoteServiceName
        );

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<RedisDistributedEventBusHttpApiClientModule>();
        });

    }
}
