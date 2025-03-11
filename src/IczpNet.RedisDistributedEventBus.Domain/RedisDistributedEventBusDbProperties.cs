namespace IczpNet.RedisDistributedEventBus;

public static class RedisDistributedEventBusDbProperties
{
    public static string DbTablePrefix { get; set; } = "RedisDistributedEventBus";

    public static string? DbSchema { get; set; } = null;

    public const string ConnectionStringName = "RedisDistributedEventBus";
}
