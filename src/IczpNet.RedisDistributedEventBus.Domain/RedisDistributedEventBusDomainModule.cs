using IczpNet.RedisDistributedEventBus.EventBus;
using IczpNet.RedisDistributedEventBus.Options;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Domain;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Modularity;

namespace IczpNet.RedisDistributedEventBus;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(AbpEventBusModule),
    typeof(RedisDistributedEventBusDomainSharedModule)
)]
public class RedisDistributedEventBusDomainModule : AbpModule
{

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        //Configure<RedisDistributedEventBusOptions>(configuration.GetSection("RabbitMQ:EventBus"));

        context.Services.AddSingleton<IDistributedEventBus, EventBus.RedisDistributedEventBus>();
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        context
            .ServiceProvider
            .GetRequiredService<IRedisDistributedEventBus>()
            .Initialize();
    }
}
