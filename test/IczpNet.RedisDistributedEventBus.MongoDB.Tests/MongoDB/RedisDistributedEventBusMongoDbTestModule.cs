using System;
using Volo.Abp.Data;
using Volo.Abp.Modularity;
using Volo.Abp.Uow;

namespace IczpNet.RedisDistributedEventBus.MongoDB;

[DependsOn(
    typeof(RedisDistributedEventBusApplicationTestModule),
    typeof(RedisDistributedEventBusMongoDbModule)
)]
public class RedisDistributedEventBusMongoDbTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpDbConnectionOptions>(options =>
        {
            options.ConnectionStrings.Default = MongoDbFixture.GetRandomConnectionString();
        });
    }
}
