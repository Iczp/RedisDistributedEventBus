using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace IczpNet.RedisDistributedEventBus;

[DependsOn(
    typeof(AbpVirtualFileSystemModule)
    )]
public class RedisDistributedEventBusInstallerModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<RedisDistributedEventBusInstallerModule>();
        });
    }
}
