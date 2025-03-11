using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace IczpNet.RedisDistributedEventBus.EntityFrameworkCore;

[ConnectionStringName(RedisDistributedEventBusDbProperties.ConnectionStringName)]
public interface IRedisDistributedEventBusDbContext : IEfCoreDbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * DbSet<Question> Questions { get; }
     */
}
