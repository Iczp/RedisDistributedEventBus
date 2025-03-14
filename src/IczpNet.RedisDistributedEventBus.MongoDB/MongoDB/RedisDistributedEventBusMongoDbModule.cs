﻿using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using Volo.Abp.MongoDB;

namespace IczpNet.RedisDistributedEventBus.MongoDB;

[DependsOn(
    typeof(RedisDistributedEventBusDomainModule),
    typeof(AbpMongoDbModule)
    )]
public class RedisDistributedEventBusMongoDbModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMongoDbContext<RedisDistributedEventBusMongoDbContext>(options =>
        {
                /* Add custom repositories here. Example:
                 * options.AddRepository<Question, MongoQuestionRepository>();
                 */
        });
    }
}
