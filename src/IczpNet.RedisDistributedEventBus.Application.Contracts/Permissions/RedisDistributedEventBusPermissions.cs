using Volo.Abp.Reflection;

namespace IczpNet.RedisDistributedEventBus.Permissions;

public class RedisDistributedEventBusPermissions
{
    public const string GroupName = "RedisDistributedEventBus";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(RedisDistributedEventBusPermissions));
    }
}
