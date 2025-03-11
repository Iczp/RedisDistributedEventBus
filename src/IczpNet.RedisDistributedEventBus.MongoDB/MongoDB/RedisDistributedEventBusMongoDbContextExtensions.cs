using Volo.Abp;
using Volo.Abp.MongoDB;

namespace IczpNet.RedisDistributedEventBus.MongoDB;

public static class RedisDistributedEventBusMongoDbContextExtensions
{
    public static void ConfigureRedisDistributedEventBus(
        this IMongoModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));
    }
}
