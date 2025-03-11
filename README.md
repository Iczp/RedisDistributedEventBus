# IczpNet.LogManagement

An abp module that provides standard tree structure entity implement.

### Create project by Abp Cli

```
abp new IczpNet.OpenIddict -t module --no-ui -v 7.3.0
```



### Build

```
dotnet build --configuration Release
```

https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-build

### Public to Nuget.org

```
dotnet nuget push "src/*/bin/Release/*0.1.2.nupkg" --skip-duplicate -k {APIKEY} --source https://api.nuget.org/v3/index.json
```

## Dependency

### Volo.Abp.Identity

```bash
abp add-module Volo.Abp.Identity
```



### Volo.AuditLogging

```bash
abp add-module Volo.AuditLogging
```

## Usage

### Api : `xxx.HttpApi.Host/xxHttpApiHostModule`

```c#
 Configure<AbpAspNetCoreMvcOptions>(options =>
 {
     options.ConventionalControllers.Create(typeof(LogManagementApplicationModule).Assembly);
 });
```



## Installation

#### Install the following NuGet packages. (see how)

- IczpNet.LogManagement.Domain
- IczpNet.LogManagement.Application
- IczpNet.LogManagement.Application.Contracts
- IczpNet.LogManagement.Domain.Shared

#### Add `DependsOn(typeof(LogManagementXxxModule))` attribute to configure the module dependencies. 

1. ### `IczpNet.LogManagementDemo.Domain` 

   `F:\Dev\abpvnext\Iczp.LogManagement\Example\src\IczpNet.LogManagementDemo.Domain\LogManagementDemoDomainModule.cs`

   ```c#
   [DependsOn(typeof(LogManagementDomainModule))]
   ```

2. ###  `IczpNet.LogManagementDemo.Domain.Shared`

   ```c#
   [DependsOn(typeof(LogManagementDomainSharedModule))]
   ```


5. ### `IczpNet.LogManagementDemo.Application.Contracts`

   ```c#
   [DependsOn(typeof(LogManagementApplicationContractsModule))]
   ```

6. ###  `IczpNet.LogManagementDemo.Application`

   ```c#
   [DependsOn(typeof(LogManagementApplicationModule))]
   ```

   

## Internal structure

### IczpNet.LogManagement.Domain

### IczpNet.LogManagement.Application.Contracts

### Dtos

#### `IAuditLogAppService`

```C#
using IczpNet.LogManagement.AuditLogs.Dtos;
using System;
using Volo.Abp.Application.Services;

namespace IczpNet.LogManagement.AuditLogs;

public interface IAuditLogAppService : IReadOnlyAppService<AuditLogDetailDto, AuditLogDto, Guid, AuditLogGetListInput>
{

}

```

#### `AuditLogDto`

```C#
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace IczpNet.LogManagement.AuditLogs.Dtos;

public class AuditLogDto : EntityDto<Guid>
{
    public virtual string ApplicationName { get; set; }

    public virtual Guid? UserId { get; set; }

    public virtual string UserName { get; set; }

    public virtual Guid? TenantId { get; set; }

    public virtual string TenantName { get; set; }

    public virtual DateTime ExecutionTime { get; set; }

    public virtual int ExecutionDuration { get; set; }

    public virtual string ClientIpAddress { get; set; }

    public virtual string ClientName { get; set; }

    public virtual string ClientId { get; set; }

    public virtual string BrowserInfo { get; set; }

    public virtual string HttpMethod { get; set; }

    public virtual string Url { get; set; }

    public virtual int? HttpStatusCode { get; set; }
}

```

#### `AuditLogDetailDto`

```C#
using System;
using Volo.Abp.Data;

namespace IczpNet.LogManagement.AuditLogs.Dtos;

public class AuditLogDetailDto : AuditLogDto, IHasExtraProperties
{
    public virtual Guid? ImpersonatorUserId { get; set; }

    public virtual string ImpersonatorUserName { get; set; }

    public virtual Guid? ImpersonatorTenantId { get; set; }

    public virtual string ImpersonatorTenantName { get; set; }

    public virtual string CorrelationId { get; set; }

    public virtual string Comments { get; set; }

    public virtual string Exceptions { get; set; }

    public ExtraPropertyDictionary ExtraProperties { get; set; }
}

```

#### ICurrentUserSecurityLogAppService

```C#
using IczpNet.LogManagement.SecurityLogs.Dtos;
using System;
using Volo.Abp.Application.Services;

namespace IczpNet.LogManagement.SecurityLogs;

public interface ICurrentUserSecurityLogAppService : IReadOnlyAppService<SecurityLogDetailDto, SecurityLogDto, Guid, CurrentUserSecurityLogGetListInput>
{

}

```

#### ISecurityLogAppService

```C#
using IczpNet.LogManagement.SecurityLogs.Dtos;
using System;
using Volo.Abp.Application.Services;

namespace IczpNet.LogManagement.SecurityLogs;

public interface ISecurityLogAppService : IReadOnlyAppService<SecurityLogDetailDto, SecurityLogDto, Guid, SecurityLogGetListInput>
{

}

```



#### SecurityLogs

```C#
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace IczpNet.LogManagement.SecurityLogs.Dtos;

public class SecurityLogDto : EntityDto<Guid>
{
    public Guid? TenantId { get; set; }

    public string ApplicationName { get; set; }

    public string Identity { get; set; }

    public string Action { get; set; }

    public Guid? UserId { get; set; }

    public string UserName { get; set; }

    public string TenantName { get; set; }

    public string ClientId { get; set; }

    public string CorrelationId { get; set; }

    public string ClientIpAddress { get; set; }

    public string BrowserInfo { get; set; }

    public DateTime CreationTime { get; set; }
}

```



### IczpNet.LogManagement.Application

#### `AuditLogAppService`

```C#
using IczpNet.LogManagement.AuditLogs.Dtos;
using IczpNet.LogManagement.BaseAppServices;
using IczpNet.LogManagement.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.AuditLogging;

namespace IczpNet.LogManagement.AuditLogs;

public class AuditLogAppService : BaseGetListAppService<AuditLog, AuditLogDetailDto, AuditLogDto, Guid, AuditLogGetListInput>, IAuditLogAppService
{
    protected override string GetListPolicyName { get; set; } = LogManagementPermissions.AuditLogPermissions.GetList;
    protected override string GetPolicyName { get; set; } = LogManagementPermissions.AuditLogPermissions.GetItem;

    public AuditLogAppService(IAuditLogRepository repository) : base(repository)
    {
    }

    //[HttpGet]
    protected override async Task<IQueryable<AuditLog>> CreateFilteredQueryAsync(AuditLogGetListInput input)
    {
        var query = (await base.CreateFilteredQueryAsync(input))
            .WhereIf(input.UserId.HasValue, x => x.UserId == input.UserId)
            .WhereIf(!string.IsNullOrWhiteSpace(input.UserName), x => x.UserName == input.UserName)
            .WhereIf(input.TenantId.HasValue, x => x.TenantId == input.TenantId)
            .WhereIf(!string.IsNullOrWhiteSpace(input.TenantName), x => x.TenantName == input.TenantName)
            .WhereIf(input.ImpersonatorTenantId.HasValue, x => x.ImpersonatorTenantId == input.ImpersonatorTenantId)
            .WhereIf(!string.IsNullOrWhiteSpace(input.ImpersonatorTenantName), x => x.ImpersonatorTenantName == input.ImpersonatorTenantName)
            .WhereIf(input.ImpersonatorUserId.HasValue, x => x.ImpersonatorUserId == input.ImpersonatorUserId)
            .WhereIf(!string.IsNullOrWhiteSpace(input.ImpersonatorUserName), x => x.ImpersonatorUserName == input.ImpersonatorUserName)
            .WhereIf(!string.IsNullOrWhiteSpace(input.CorrelationId), x => x.CorrelationId == input.CorrelationId)
            .WhereIf(!string.IsNullOrWhiteSpace(input.ClientId), x => x.ClientId == input.ClientId)
            .WhereIf(!string.IsNullOrWhiteSpace(input.ClientName), x => x.ClientName == input.ClientName)
            .WhereIf(!string.IsNullOrWhiteSpace(input.ClientIpAddress), x => x.ClientIpAddress == input.ClientIpAddress)
            .WhereIf(!string.IsNullOrWhiteSpace(input.ApplicationName), x => x.ApplicationName == input.ApplicationName)
            .WhereIf(input.HttpMethods != null && input.HttpMethods.Count != 0, x => input.HttpMethods!.Contains(x.HttpMethod))
            .WhereIf(input.HttpStatusCodes != null && input.HttpStatusCodes.Count != 0, x => input.HttpStatusCodes!.Contains(x.HttpStatusCode))

            .WhereIf(input.MinExecutionDuration.HasValue, x => x.ExecutionDuration > input.MinExecutionDuration)
            .WhereIf(input.MaxExecutionDuration.HasValue, x => x.ExecutionDuration <= input.MaxExecutionDuration)

            .WhereIf(input.StartExecutionTime.HasValue, x => x.ExecutionTime > input.StartExecutionTime)
            .WhereIf(input.EndExecutionTime.HasValue, x => x.ExecutionTime <= input.EndExecutionTime)

            .WhereIf(string.IsNullOrWhiteSpace(input.Comments), x => x.Comments.Contains(input.Comments))
            .WhereIf(string.IsNullOrWhiteSpace(input.BrowserInfo), x => x.BrowserInfo.Contains(input.BrowserInfo))
        ;

        return query;
    }

    protected override IQueryable<AuditLog> ApplyDefaultSorting(IQueryable<AuditLog> query)
    {
        return query.OrderByDescending(x => x.ExecutionTime);
    }
}

```

#### `SecurityLogAppService`

```C#
using IczpNet.LogManagement.SecurityLogs.Dtos;
using IczpNet.LogManagement.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Identity;
using Volo.Abp.Domain.Repositories;
using IczpNet.LogManagement.BaseAppServices;

namespace IczpNet.LogManagement.SecurityLogs;

public class SecurityLogAppService : BaseGetListAppService<IdentitySecurityLog, SecurityLogDetailDto, SecurityLogDto, Guid, SecurityLogGetListInput>, ISecurityLogAppService
{
    protected override string GetListPolicyName { get; set; } = LogManagementPermissions.SecurityLogPermissions.GetList;
    protected override string GetPolicyName { get; set; } = LogManagementPermissions.SecurityLogPermissions.GetItem;

    public SecurityLogAppService(IRepository<IdentitySecurityLog, Guid> repository) : base(repository)
    {
       
    }

    protected override async Task<IQueryable<IdentitySecurityLog>> CreateFilteredQueryAsync(SecurityLogGetListInput input)
    {
        var query = (await base.CreateFilteredQueryAsync(input))
            .WhereIf(!string.IsNullOrWhiteSpace(input.Identity), x => x.Identity == input.Identity)
            .WhereIf(input.UserId.HasValue, x => x.UserId == input.UserId)
            .WhereIf(!string.IsNullOrWhiteSpace(input.UserName), x => x.UserName == input.UserName)
            .WhereIf(input.TenantId.HasValue, x => x.TenantId == input.TenantId)
            .WhereIf(!string.IsNullOrWhiteSpace(input.TenantName), x => x.TenantName == input.TenantName)
            .WhereIf(!string.IsNullOrWhiteSpace(input.CorrelationId), x => x.CorrelationId == input.CorrelationId)
            .WhereIf(!string.IsNullOrWhiteSpace(input.ClientId), x => x.ClientId == input.ClientId)
            .WhereIf(!string.IsNullOrWhiteSpace(input.ClientIpAddress), x => x.ClientIpAddress == input.ClientIpAddress)
            .WhereIf(!string.IsNullOrWhiteSpace(input.ApplicationName), x => x.ApplicationName == input.ApplicationName)
            .WhereIf(input.Actions != null && input.Actions.Count != 0, x => input.Actions!.Contains(x.Action))
            .WhereIf(input.StartCreationTime.HasValue, x => x.CreationTime > input.StartCreationTime)
            .WhereIf(input.EndCreationTime.HasValue, x => x.CreationTime <= input.EndCreationTime)
            .WhereIf(!string.IsNullOrWhiteSpace(input.BrowserInfo), x => x.BrowserInfo.Contains(input.BrowserInfo))
        ;

        return query;
    }

    protected override IQueryable<IdentitySecurityLog> ApplyDefaultSorting(IQueryable<IdentitySecurityLog> query)
    {
        return query.OrderByDescending(x => x.CreationTime);
        //return base.ApplyDefaultSorting(query);
    }
}

```

#### `CurrentUserSecurityLogAppService`

```C#
using IczpNet.LogManagement.SecurityLogs.Dtos;
using IczpNet.LogManagement.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Identity;
using Volo.Abp.Domain.Repositories;
using IczpNet.LogManagement.BaseAppServices;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Auditing;

namespace IczpNet.LogManagement.SecurityLogs;

public class CurrentUserSecurityLogAppService : BaseGetListAppService<IdentitySecurityLog, SecurityLogDetailDto, SecurityLogDto, Guid, CurrentUserSecurityLogGetListInput>, ICurrentUserSecurityLogAppService
{
    protected override string GetListPolicyName { get; set; } = LogManagementPermissions.CurrentUserSecurityLogPermissions.GetList;
    protected override string GetPolicyName { get; set; } = LogManagementPermissions.CurrentUserSecurityLogPermissions.GetItem;

    public CurrentUserSecurityLogAppService(IRepository<IdentitySecurityLog, Guid> repository) : base(repository)
    {

    }

    protected override async Task<IQueryable<IdentitySecurityLog>> CreateFilteredQueryAsync(CurrentUserSecurityLogGetListInput input)
    {
        var query = (await base.CreateFilteredQueryAsync(input))
            .Where(x => x.TenantId == CurrentUser.TenantId)
            //.Where(x => x.UserId == CurrentUser.Id)
            .Where(x => x.UserName == CurrentUser.UserName)
            .WhereIf(!string.IsNullOrWhiteSpace(input.Identity), x => x.Identity == input.Identity)
            .WhereIf(!string.IsNullOrWhiteSpace(input.CorrelationId), x => x.CorrelationId == input.CorrelationId)
            .WhereIf(!string.IsNullOrWhiteSpace(input.ClientId), x => x.ClientId == input.ClientId)
            .WhereIf(!string.IsNullOrWhiteSpace(input.ClientIpAddress), x => x.ClientIpAddress == input.ClientIpAddress)
            .WhereIf(!string.IsNullOrWhiteSpace(input.ApplicationName), x => x.ApplicationName == input.ApplicationName)
            .WhereIf(input.Actions != null && input.Actions.Count != 0, x => input.Actions!.Contains(x.Action))
            .WhereIf(input.StartCreationTime.HasValue, x => x.CreationTime > input.StartCreationTime)
            .WhereIf(input.EndCreationTime.HasValue, x => x.CreationTime <= input.EndCreationTime)
            .WhereIf(!string.IsNullOrWhiteSpace(input.BrowserInfo), x => x.BrowserInfo.Contains(input.BrowserInfo))
        ;

        return query;
    }

    protected override async Task<IdentitySecurityLog> GetEntityByIdAsync(Guid id)
    {
        var entity = await base.GetEntityByIdAsync(id);

        //if (entity.UserId != CurrentUser.Id)
        if (entity.UserName != CurrentUser.UserName)
        {
            throw new EntityNotFoundException($"Not such entity by current user['{CurrentUser.Name}'],Entity id:{id}");
        }
        return await base.GetEntityByIdAsync(id);
    }

    protected override IQueryable<IdentitySecurityLog> ApplyDefaultSorting(IQueryable<IdentitySecurityLog> query)
    {
        return query.OrderByDescending(x => x.CreationTime);
        //return base.ApplyDefaultSorting(query);
    }
}

```

### AutoMappers

#### LogManagementApplicationAutoMapperProfile

```C#
using AutoMapper;
using IczpNet.LogManagement.AuditLogs.Dtos;
using IczpNet.LogManagement.SecurityLogs.Dtos;
using Volo.Abp.AuditLogging;
using Volo.Abp.Identity;

namespace IczpNet.LogManagement.AutoMappers;

public class LogManagementApplicationAutoMapperProfile : Profile
{
    public LogManagementApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        //AuditLog
        CreateMap<AuditLog, AuditLogDto>();
        CreateMap<AuditLog, AuditLogDetailDto>()
            .MapExtraProperties();

        //SecurityLog
        CreateMap<IdentitySecurityLog, SecurityLogDto>();
        CreateMap<IdentitySecurityLog, SecurityLogDetailDto>()
            .MapExtraProperties();
    }
}

```

