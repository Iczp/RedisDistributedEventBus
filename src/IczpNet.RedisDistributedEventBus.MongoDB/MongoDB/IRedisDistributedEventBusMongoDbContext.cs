using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace IczpNet.RedisDistributedEventBus.MongoDB;

[ConnectionStringName(RedisDistributedEventBusDbProperties.ConnectionStringName)]
public interface IRedisDistributedEventBusMongoDbContext : IAbpMongoDbContext
{
    /* Define mongo collections here. Example:
     * IMongoCollection<Question> Questions { get; }
     */
}
