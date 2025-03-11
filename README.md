# IczpNet.RedisDistributedEventBus

用Redis的发布与订阅 实现Abp分布式事件



## Distributed Event Bus Redis Integration

本文档解释如何配置Redis作为分布式事件总线提供程序，使用IczpNet.RedisDistributedEventBus。请参阅分布式事件总线文档以了解如何使用分布式事件总线系统。

### 安装

使用NuGet包管理器将IczpNet.RedisDistributedEventBus包添加到您的项目中。

如果尚未安装NuGet包管理器，请先安装。

在Visual Studio中，打开您的项目。

在“解决方案资源管理器”中，右键点击您的项目，选择“管理NuGet包”。

在NuGet包管理器中，搜索“IczpNet.RedisDistributedEventBus”并安装。

或者，您可以使用命令行安装：


```bash
dotnet add package IczpNet.RedisDistributedEventBus
```

如果您使用的是ABP框架，您还需要将IczpNet.RedisDistributedEventBus包添加到您的模块依赖中：

```csharp
[DependsOn(typeof(RedisDistributedEventBusModule))]
public class YourModule : AbpModule
{
    // 模块的其他配置
}
```

### 配置

您可以使用标准配置系统进行配置，例如使用appsettings.json文件或使用选项类。

`appsettings.json`文件配置
这是配置Redis设置的最简单方法。它也非常强大，因为您可以使用ASP.NET Core支持的任何其他配置源（如环境变量）。

示例：使用默认配置连接到本地Redis服务器的最小配置

```json
{
  "Redis": {
    "Configuration": "127.0.0.1",
    "PrefixKey": "IczpNetEventBus:"
  }
}
```

PrefixKey是此应用程序的事件总线通道名称前缀。

Configuration是Redis服务器的地址。


ConnectionString可以是任何有效的Redis连接字符串，包括主机名、端口、密码等。

### 选项类

RedisEventBusOptions类可用于配置Redis事件总线的选项。

```c#
public class RedisDistributedEventBusOptions
{
    public string PrifixKey { get; set; } = "IczpNet.RedisDistributedEventBus";
}

```

### 订阅事件  实现接口： `IDistributedEventHandler<SampleDto>`

```c#
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;

namespace IczpNet.RedisDistributedEventBus.Samples;

public class SampleDistributedEventHandler : DomainService, IDistributedEventHandler<SampleDto>, ITransientDependency
{
    public async Task HandleEventAsync(SampleDto eventData)
    {
        Logger.LogWarning($"Handled SampleDto: {eventData.Value}");

        await Task.CompletedTask;
    }
}

```

在这个示例中，MyEventHandler实现了`IDistributedEventHandler<SampleDto>`接口，用于处理MyEvent事件。PublishEventAsync方法演示了如何发布事件。



### 发布事件 await DistributedEventBus.PublishAsync(ret)

```C#
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.EventBus.Distributed;

namespace IczpNet.RedisDistributedEventBus.Samples;

[Area(RedisDistributedEventBusRemoteServiceConsts.ModuleName)]
[RemoteService(Name = RedisDistributedEventBusRemoteServiceConsts.RemoteServiceName)]
[Route("api/RedisDistributedEventBus/sample")]
public class SampleController(
    ISampleAppService sampleAppService,
    IDistributedEventBus distributedEventBus
    ) : RedisDistributedEventBusController, ISampleAppService
{
    private readonly ISampleAppService _sampleAppService = sampleAppService;

    public IDistributedEventBus DistributedEventBus { get; } = distributedEventBus;

    [HttpPost]
    public async Task<SampleDto> PostAsync()
    {
        var ret = new SampleDto()
        {
            Value = DateTime.Now.Ticks
        };
        await DistributedEventBus.PublishAsync(ret); //发布事件 
        return ret;

    }

}

```



