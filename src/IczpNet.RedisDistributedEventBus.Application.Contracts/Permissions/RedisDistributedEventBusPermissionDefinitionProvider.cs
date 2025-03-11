using IczpNet.RedisDistributedEventBus.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace IczpNet.RedisDistributedEventBus.Permissions;

public class RedisDistributedEventBusPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(RedisDistributedEventBusPermissions.GroupName, L("Permission:RedisDistributedEventBus"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<RedisDistributedEventBusResource>(name);
    }
}
