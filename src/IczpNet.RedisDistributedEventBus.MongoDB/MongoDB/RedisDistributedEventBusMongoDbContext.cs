using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace IczpNet.RedisDistributedEventBus.MongoDB;

[ConnectionStringName(RedisDistributedEventBusDbProperties.ConnectionStringName)]
public class RedisDistributedEventBusMongoDbContext : AbpMongoDbContext, IRedisDistributedEventBusMongoDbContext
{
    /* Add mongo collections here. Example:
     * public IMongoCollection<Question> Questions => Collection<Question>();
     */

    protected override void CreateModel(IMongoModelBuilder modelBuilder)
    {
        base.CreateModel(modelBuilder);

        modelBuilder.ConfigureRedisDistributedEventBus();
    }
}
