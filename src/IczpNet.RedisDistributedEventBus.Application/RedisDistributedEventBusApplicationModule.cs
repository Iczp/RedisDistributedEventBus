using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.Application;

namespace IczpNet.RedisDistributedEventBus;

[DependsOn(
    typeof(RedisDistributedEventBusDomainModule),
    typeof(RedisDistributedEventBusApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpAutoMapperModule)
    )]
public class RedisDistributedEventBusApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapperObjectMapper<RedisDistributedEventBusApplicationModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<RedisDistributedEventBusApplicationModule>(validate: true);
        });
    }
}
