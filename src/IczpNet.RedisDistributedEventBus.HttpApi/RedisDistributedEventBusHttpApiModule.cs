using Localization.Resources.AbpUi;
using IczpNet.RedisDistributedEventBus.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;

namespace IczpNet.RedisDistributedEventBus;

[DependsOn(
    typeof(RedisDistributedEventBusApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class RedisDistributedEventBusHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(RedisDistributedEventBusHttpApiModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<RedisDistributedEventBusResource>()
                .AddBaseTypes(typeof(AbpUiResource));
        });
    }
}
