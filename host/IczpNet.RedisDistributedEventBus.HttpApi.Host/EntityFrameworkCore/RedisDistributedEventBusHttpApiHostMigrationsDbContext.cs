using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace IczpNet.RedisDistributedEventBus.EntityFrameworkCore;

public class RedisDistributedEventBusHttpApiHostMigrationsDbContext : AbpDbContext<RedisDistributedEventBusHttpApiHostMigrationsDbContext>
{
    public RedisDistributedEventBusHttpApiHostMigrationsDbContext(DbContextOptions<RedisDistributedEventBusHttpApiHostMigrationsDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ConfigureRedisDistributedEventBus();
    }
}
