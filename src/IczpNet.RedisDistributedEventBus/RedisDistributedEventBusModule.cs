using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Volo.Abp;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Modularity;

namespace IczpNet.RedisDistributedEventBus;

[DependsOn(typeof(AbpEventBusModule))]

public class RedisDistributedEventBusModule : AbpModule
{

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        Configure<RedisDistributedEventBusOptions>(configuration.GetSection("Redis"));

        // 配置 Redis 连接
        //var configuration = ConfigurationOptions.Parse("localhost"); // 替换为你的 Redis 连接字符串
        var redisConnection = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]!);

        // 注册 IConnectionMultiplexer
        context.Services.AddSingleton<IConnectionMultiplexer>(redisConnection);

        context.Services.AddSingleton<IDistributedEventBus, RedisDistributedEventBus>();
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        context
            .ServiceProvider
            .GetRequiredService<IRedisDistributedEventBus>()
            .Initialize();
    }
}
