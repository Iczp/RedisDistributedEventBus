using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace IczpNet.RedisDistributedEventBus.EntityFrameworkCore;

[ConnectionStringName(RedisDistributedEventBusDbProperties.ConnectionStringName)]
public class RedisDistributedEventBusDbContext : AbpDbContext<RedisDistributedEventBusDbContext>, IRedisDistributedEventBusDbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * public DbSet<Question> Questions { get; set; }
     */

    public RedisDistributedEventBusDbContext(DbContextOptions<RedisDistributedEventBusDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureRedisDistributedEventBus();
    }
}
