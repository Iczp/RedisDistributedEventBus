using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace IczpNet.RedisDistributedEventBus.EntityFrameworkCore;

public class RedisDistributedEventBusHttpApiHostMigrationsDbContextFactory : IDesignTimeDbContextFactory<RedisDistributedEventBusHttpApiHostMigrationsDbContext>
{
    public RedisDistributedEventBusHttpApiHostMigrationsDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();

        var builder = new DbContextOptionsBuilder<RedisDistributedEventBusHttpApiHostMigrationsDbContext>()
            .UseSqlServer(configuration.GetConnectionString("RedisDistributedEventBus"));

        return new RedisDistributedEventBusHttpApiHostMigrationsDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
