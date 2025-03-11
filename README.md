



# IczpNet.RedisDistributedEventBus

用Redis的发布与订阅 实现Abp分布式事件



https://abp.io/docs/latest/framework/infrastructure/event-bus/distributed



# 分布式事件总线

分布式事件总线系统允许**发布**和**订阅**可以举办的活动**跨应用程序/服务边界传输**。您可以使用分布式事件总线在**微服务**或者**应用程序**。

## 提供者

分布式事件总线系统提供了**抽象**任何供应商/提供商都可以实现。有四种现成的提供商：

- `LocalDistributedEventBus`是实现分布式事件总线以进程内方式工作的默认实现。是的！**[默认实现与本地事件总线](https://abp.io/docs/latest/framework/infrastructure/event-bus/local)一样**，如果您没有配置真正的分布式提供程序。
- `AzureDistributedEventBus`[使用Azure 服务总线](https://azure.microsoft.com/en-us/services/service-bus/)实现分布式事件总线。请参阅[Azure 服务总线集成文档](https://abp.io/docs/latest/framework/infrastructure/event-bus/distributed/azure)以了解如何配置它。
- `RabbitMqDistributedEventBus`[使用RabbitMQ](https://www.rabbitmq.com/)实现分布式事件总线。请参阅[RabbitMQ 集成文档](https://abp.io/docs/latest/framework/infrastructure/event-bus/distributed/rabbitmq)以了解如何配置它。
- `KafkaDistributedEventBus`[使用Kafka](https://kafka.apache.org/)实现分布式事件总线。请参阅[Kafka 集成文档](https://abp.io/docs/latest/framework/infrastructure/event-bus/distributed/kafka)以了解如何配置它。
- `RebusDistributedEventBus`[使用Rebus](http://mookid.dk/category/rebus/)实现分布式事件总线。请参阅[Rebus 集成文档](https://abp.io/docs/latest/framework/infrastructure/event-bus/distributed/rebus)以了解如何配置它。
- `RedisDistributedEventBus` 使用 Redis 实现分布式事件总线

默认使用本地事件总线有几个重要的优点。最重要的一点是：它允许您编写与分布式架构兼容的代码。您现在可以编写一个单片应用程序，以后可以将其拆分为微服务。通过分布式事件而不是本地事件在有界上下文之间（或应用程序模块之间）进行通信是一种很好的做法。

例如，[预构建的应用程序模块](https://abp.io/docs/latest/modules)被设计为分布式系统中的服务，而它们也可以作为单片应用程序中的模块工作，而不依赖于外部消息代理。







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

```C#
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

### 订阅事件  



服务可以实现来`IDistributedEventHandler<TEvent>`处理事件。

**示例：处理`StockCountChangedEto`上面定义的**

```C#
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace AbpDemo
{
    public class MyHandler
        : IDistributedEventHandler<StockCountChangedEto>,
          ITransientDependency
    {
        public async Task HandleEventAsync(StockCountChangedEto eventData)
        {
            var productId = eventData.ProductId;
        }
    }
}
```

就这样。

- `MyHandler`是**自动发现**由 ABP 执行，`HandleEventAsync`每当有事件发生时就会被调用`StockCountChangedEto`。
- 如果你正在使用分布式消息代理，比如 RabbitMQ，ABP 会自动**订阅消息代理上的事件**，获取消息，执行处理程序。
- 它发送**确认 (ACK)**如果事件处理程序成功执行（没有引发任何异常），则发送给消息代理。

您可以在此处注入任何服务并执行任何所需的逻辑。单个事件处理程序类可以**订阅多个事件**`IDistributedEventHandler<TEvent>`但要为每种事件类型实现接口。

如果你执行**数据库操作**并在事件处理程序中使用[存储库](https://abp.io/docs/latest/framework/architecture/domain-driven-design/repositories)，您可能需要创建一个[工作单元](https://abp.io/docs/latest/framework/architecture/domain-driven-design/unit-of-work)，因为某些存储库方法需要在**活动工作单元**。创建 handle 方法并为该方法`virtual`添加属性，或者手动使用创建工作单元范围。`[UnitOfWork]``IUnitOfWorkManager`

> 处理程序类必须注册到依赖项注入 (DI)。上面的示例使用`ITransientDependency`来完成此操作。请参阅[DI 文档](https://abp.io/docs/latest/framework/fundamentals/dependency-injection)以了解更多选项。



实现接口： `IDistributedEventHandler<SampleDto>`

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



### 发布事件  

###  使用 IDistributedEventBus 发布事件

`IDistributedEventBus`可以被[注入](https://abp.io/docs/latest/framework/fundamentals/dependency-injection)并用于发布分布式事件。

**示例：当产品库存数量发生变化时发布分布式事件**

```C#
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace AbpDemo
{
    public class MyService : ITransientDependency
    {
        private readonly IDistributedEventBus _distributedEventBus;

        public MyService(IDistributedEventBus distributedEventBus)
        {
            _distributedEventBus = distributedEventBus;
        }
        
        public virtual async Task ChangeStockCountAsync(Guid productId, int newCount)
        {
            await _distributedEventBus.PublishAsync(
                new StockCountChangedEto
                {
                    ProductId = productId,
                    NewCount = newCount
                }
            );
        }
    }
}

```

`PublishAsync`方法获取事件对象，该对象负责保存与事件相关的数据。它是一个简单的普通类：

```C#
using System;

namespace AbpDemo
{
    [EventName("MyApp.Product.StockChange")]
    public class StockCountChangedEto
    {
        public Guid ProductId { get; set; }
        
        public int NewCount { get; set; }
    }
}
```

即使您不需要传输任何数据，您也需要创建一个类（在本例中是一个空类）。

> `Eto`是后缀**埃**发泄**电视**转移**哦**我们习惯使用的对象。虽然这不是必需的，但我们发现识别此类事件类很有用（就像应用程序层上的[DTO一样）。](https://abp.io/docs/latest/framework/architecture/domain-driven-design/data-transfer-objects)

#### 事件名称

`EventName`属性是可选的，但建议使用。如果您没有为事件类型（ETO 类）声明它，则事件名称将是事件类的全名（`AbpDemo.StockCountChangedEto`在本例中）。

#### 关于事件对象的序列化

事件传输对象 (ETO)**必须可序列化**因为当它们被传输到进程外时，它们将被序列化/反序列化为 JSON 或其他格式。

避免循环引用，多态性，私有设置器，如果有任何其他构造函数，则提供默认（空）构造函数，这是一种很好的做法（虽然一些序列化器可能会容忍它），就像 DTO 一样。







另一个例子：

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



### 在实体/聚合根类中发布事件

[实体](https://abp.io/docs/latest/framework/architecture/domain-driven-design/entities)不能通过依赖注入来注入服务，但是在实体/聚合根类内发布分布式事件是很常见的。

**示例：在聚合根方法内发布分布式事件**



```C#
using System;
using Volo.Abp.Domain.Entities;

namespace AbpDemo
{
    public class Product : AggregateRoot<Guid>
    {
        public string Name { get; set; }
        
        public int StockCount { get; private set; }

        private Product() { }

        public Product(Guid id, string name)
            : base(id)
        {
            Name = name;
        }

        public void ChangeStockCount(int newCount)
        {
            StockCount = newCount;
            
            //ADD an EVENT TO BE PUBLISHED
            AddDistributedEvent(
                new StockCountChangedEto
                {
                    ProductId = Id,
                    NewCount = newCount
                }
            );
        }
    }
}
```

`AggregateRoot`类定义`AddDistributedEvent`添加新的分布式事件，当聚合根对象保存（创建、更新或删除）到数据库时发布该事件。

> 如果实体发布了这样的事件，那么以受控的方式更改相关属性是一种很好的做法，就像上面的例子一样 -`StockCount`只能通过`ChangeStockCount`保证发布事件的方法进行更改。

#### IGeneratesDomainEvents 接口

实际上，添加分布式事件并不是`AggregateRoot`类所独有的。您可以`IGeneratesDomainEvents`为任何实体类实现它。但是，`AggregateRoot`默认情况下会实现它，这让您更容易实现。

> 不建议为非聚合根的实体实现此接口，因为它可能不适用于此类实体的某些数据库提供程序。例如，它适用于 EF Core，但不适用于 MongoDB。

#### 它是如何实现的？

调用`AddDistributedEvent`不会立即发布事件。当您将更改保存到数据库时，才会发布事件；

- 对于 EF Core，它发布于`DbContext.SaveChanges`。
- 对于 MongoDB，当您调用存储库的`InsertAsync`、`UpdateAsync`或`DeleteAsync`方法时它会被发布（因为 MongoDB 没有变更跟踪系统）。



## 监控分布式事件

ABP 可让您随时了解申请情况**接收**或者**发送**分布式事件。此功能使您能够跟踪应用程序内的事件流，并根据接收或发送的分布式事件采取适当的操作。

### 收到的事件

`DistributedEventReceived`当您的应用程序从分布式事件总线接收到事件时，就会发布本地事件。该类`DistributedEventReceived`具有以下字段：

- **`Source`：**表示分布式事件的来源。来源可以是`Direct`、`Inbox`、`Outbox`。
- **`EventName`：**它代表收到的事件的[名称。](https://abp.io/docs/latest/framework/infrastructure/event-bus/distributed#event-name)
- **`EventData`：**它表示与收到的事件相关的实际数据。由于它是 类型`object`，因此它可以保存任何类型的数据。

**示例：当您的应用程序从分布式事件总线收到事件时收到通知**

```C#
public class DistributedEventReceivedHandler : ILocalEventHandler<DistributedEventReceived>, ITransientDependency
{
    public async Task HandleEventAsync(DistributedEventReceived eventData)
    {
        // TODO: IMPLEMENT YOUR LOGIC...
    }
}
```

### 已发送事件

`DistributedEventSent`当您的应用程序向分布式事件总线发送事件时，会发布本地事件。该类`DistributedEventSent`具有以下字段：

- **`Source`：**表示分布式事件的来源。来源可以是`Direct`、`Inbox`、`Outbox`。
- **`EventName`：**它代表发送的事件的[名称。](https://abp.io/docs/latest/framework/infrastructure/event-bus/distributed#event-name)
- **`EventData`：**它表示与发送的事件相关的实际数据。由于它是 类型`object`，因此它可以保存任何类型的数据。

**示例：当您的应用程序向分布式事件总线发送事件时收到通知**

```C#
public class DistributedEventSentHandler : ILocalEventHandler<DistributedEventSent>, ITransientDependency
{
    public async Task HandleEventAsync(DistributedEventSent eventData)
    {
        // TODO: IMPLEMENT YOUR LOGIC...
    }
}
```

您可以通过订阅`DistributedEventReceived`和`DistributedEventSent`本地事件（如上例所示）将事件跟踪功能无缝集成到您的应用程序中。这使您能够有效地监控消息传递流、诊断任何潜在问题并获得有关分布式消息传递系统行为的宝贵见解。



## 预定义事件

动态血压**自动发布**分布式事件**创建、更新和删除**配置[实体](https://abp.io/docs/latest/framework/architecture/domain-driven-design/entities)后，对其进行操作。

### 事件类型

有三种预定义的事件类型：

- `EntityCreatedEto<T>``T`在创建类型实体时发布。
- `EntityUpdatedEto<T>`当类型实体`T`更新时发布。
- `EntityDeletedEto<T>`当类型实体`T`被删除时发布。

这些类型是泛型。`T`实际上是**埃**发泄**电视**转移**哦**对象（ETO）而不是实体的类型。因为实体对象不能作为事件数据的一部分进行传输。因此，通常为实体类定义一个 ETO 类，就像`ProductEto`为`Product`实体定义一个 ETO 类一样。

### 订阅活动

订阅自动事件与订阅常规分布式事件相同。

**示例：产品更新后收到通知**



```C#
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.EventBus.Distributed;

namespace AbpDemo
{
    public class MyHandler : 
        IDistributedEventHandler<EntityUpdatedEto<ProductEto>>,
        ITransientDependency
    {
        public async Task HandleEventAsync(EntityUpdatedEto<ProductEto> eventData)
        {
            var productId = eventData.Entity.Id;
            //TODO
        }
    }
}
```

- `MyHandler`实现了`IDistributedEventHandler<EntityUpdatedEto<ProductEto>>`。
- 需要将您的处理程序类注册到[依赖项注入](https://abp.io/docs/latest/framework/fundamentals/dependency-injection)`ITransientDependency`系统。像本例中那样实现是一种简单的方法。

### 配置

`AbpDistributedEntityEventOptions`您可以在模块中`ConfigureServices`配置[以](https://abp.io/docs/latest/framework/architecture/modularity/basics)添加选择器。

**示例：配置示例**



```C#
Configure<AbpDistributedEntityEventOptions>(options =>
{
    //Enable for all entities
    options.AutoEventSelectors.AddAll();

    //Enable for a single entity
    options.AutoEventSelectors.Add<Product>();

    //Enable for all entities in a namespace (and child namespaces)
    options.AutoEventSelectors.AddNamespace("MyProject.Products");

    //Custom predicate expression that should return true to select a type
    options.AutoEventSelectors.Add(
        type => type.Namespace.StartsWith("MyProject.")
    );
});
```

- 最后一个提供了灵活性来决定是否应该为给定的实体类型发布事件。返回`true`以接受`Type`。

您可以添加多个选择器。如果其中一个选择器与实体类型匹配，则选择该实体类型。



### 事件传输对象

启用后**自动事件**对于实体，ABP 开始发布有关此实体更改的事件。如果你没有指定相应的**埃**发泄**电视**转移**哦**bject（ETO）对于实体，ABP使用一个名为的标准类型`EntityEto`，它只有两个属性：

- `EntityType`( `string`)：实体类的全名（包括命名空间）。
- `KeysAsString`( `string`)：已更改实体的主键。如果它有一个键，则此属性将为主键值。对于复合键，它将包含以`,`(逗号) 分隔的所有键。

因此，您可以实现`IDistributedEventHandler<EntityUpdatedEto<EntityEto>>`订阅更新事件。但是，订阅这种通用事件并不是一个好方法，因为您会在单个处理程序中处理所有实体的更新事件（因为它们都使用相同的 ETO 对象）。您可以为实体类型定义相应的 ETO 类型。

**示例：声明用于`ProductEto`实体`Product`**



```C#
public class ProductEto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public float Price { get; set; }
}
```

然后您可以使用该`AbpDistributedEntityEventOptions.EtoMappings`选项将您的`Product`实体映射到`ProductEto`：

```C#
Configure<AbpDistributedEntityEventOptions>(options =>
{
    options.AutoEventSelectors.Add<Product>();
    options.EtoMappings.Add<Product, ProductEto>();
});
```

这个例子；

- 添加一个选择器以允许发布实体的创建、更新和删除事件`Product`。
- 配置使用`ProductEto`作为事件传输对象来发布`Product`相关事件。

> 分布式事件系统使用[对象到对象映射](https://abp.io/docs/latest/framework/infrastructure/object-to-object-mapping)系统将对象映射`Product`到`ProductEto`对象。因此，您还需要配置对象映射（`Product`-> `ProductEto`）。您可以查看[对象到对象映射文档](https://abp.io/docs/latest/framework/infrastructure/object-to-object-mapping)以了解如何操作。



## 实体同步器

在分布式（或微服务）系统中，通常会订阅另一个服务的[实体](https://abp.io/docs/latest/framework/architecture/domain-driven-design/entities)类型的更改事件，因此当订阅的实体发生更改时，您可以收到通知。在这种情况下，您可以使用上一节中所述的 ABP 预定义事件。

如果您的目的是存储远程实体的本地副本，您通常会订阅远程实体的创建、更新和删除事件，并在事件处理程序中更新本地数据库。ABP 提供了一个预构建的`EntitySynchronizer`基类，使该操作对您来说更容易。

`Product`假设目录微服务中有一个实体（可能是聚合根实体），并且您想要使用本地`OrderProduct`实体保留订购微服务中产品的副本。实际上，`OrderProduct`类的属性将是属性的子集`Product`，因为订购微服务中不需要所有产品数据（但是，如果需要，您可以制作完整副本）。此外，`OrderProduct`实体可能具有在订购微服务中填充和使用的其他属性。

建立同步的第一步是在 Catalog 微服务中定义一个用于传输事件数据的 ETO（事件传输对象）类。假设`Product`实体有一个`Guid`键，您的 ETO 可以如下所示：

```C#
[EventName("product")]
public class ProductEto : EntityEto<Guid>
{
    // Your Product properties here...
}
```

最后，您应该在 Ordering 微服务中创建一个类，该类派生自以下`EntitySynchronizer`类：

```C#
public class ProductSynchronizer : EntitySynchronizer<OrderProduct, Guid, ProductEto>
{
    public ProductSynchronizer(
        IObjectMapper objectMapper,
        IRepository<OrderProduct, Guid> repository
        ) : base(objectMapper, repository)
    {
    }
}
```

这个类的要点是它订阅源实体的创建、更新和删除事件并更新数据库中的本地实体。它使用[对象映射器](https://abp.io/docs/latest/framework/infrastructure/object-to-object-mapping)`OrderProduct`系统从对象创建或更新对象。因此，您还应该配置对象映射器以使其正常工作。否则，您应该通过重写类中的和方法`ProductEto`来手动执行对象映射。`MapToEntityAsync(TSourceEntityEto)``MapToEntityAsync(TSourceEntityEto,TEntity)``ProductSynchronizer`

如果您的实体具有复合主键（请参阅[实体文档](https://abp.io/docs/latest/framework/architecture/domain-driven-design/entities)），那么您应该从该类继承`EntitySynchronizer<TEntity, TSourceEntityEto>`（只是不要使用`Guid`上例中的泛型参数）并实现`FindLocalEntityAsync`使用在本地数据库中查找实体`Repository`。

`EntitySynchronizer`*与实体版本控制*系统兼容（请参阅[实体文档](https://abp.io/docs/latest/framework/architecture/domain-driven-design/entities)）。因此，即使接收的事件混乱，它也能按预期工作。如果本地数据库中实体的版本比接收事件中的实体新，则忽略该事件。您应该`IHasEntityVersion`为实体和 ETO 类实现接口（对于此示例，您应该为`Product`、`ProductEto`和`OrderProduct`类实现）。

如果要忽略某些类型的更改事件，您可以在类的构造函数中设置`IgnoreEntityCreatedEvent`、`IgnoreEntityUpdatedEvent`和。例如：`IgnoreEntityDeletedEvent`



```C#
public class ProductSynchronizer 
    : EntitySynchronizer<OrderProduct, Guid, ProductEto>
{
    public ProductSynchronizer(
        IObjectMapper objectMapper,
        IRepository<OrderProduct, Guid> repository
        ) : base(objectMapper, repository)
    {
        IgnoreEntityDeletedEvent = true;
    }
}
```

> 请注意，`EntitySynchronizer`只有在您使用它之后才能创建/更新实体。如果您有一个包含现有数据的现有系统，则应该手动复制一次数据，因为`EntitySynchronizer`开始工作。

## 事务和异常处理

分布式事件总线在进程内工作（因为默认实现是`LocalDistributedEventBus`），除非您配置实际提供程序（例如[Kafka](https://abp.io/docs/latest/framework/infrastructure/event-bus/distributed/kafka)或[RabbitMQ](https://abp.io/docs/latest/framework/infrastructure/event-bus/distributed/rabbitmq)）。进程内事件总线始终在您发布事件的同一[工作单元](https://abp.io/docs/latest/framework/architecture/domain-driven-design/unit-of-work)范围内执行事件处理程序。这意味着，如果事件处理程序引发异常，则相关工作单元（数据库事务）将回滚。通过这种方式，您的应用程序逻辑和事件处理逻辑将变得事务性（原子性）和一致。如果要忽略事件处理程序中的错误，则必须`try-catch`在处理程序中使用块，并且不应重新引发异常。

当您切换到实际的分布式事件总线提供程序（例如[Kafka](https://abp.io/docs/latest/framework/infrastructure/event-bus/distributed/kafka)或[RabbitMQ](https://abp.io/docs/latest/framework/infrastructure/event-bus/distributed/rabbitmq)）时，事件处理程序将在不同的进程/应用程序中执行，因为它们的目的是创建分布式系统。在这种情况下，实现事务性事件发布的唯一方法是使用发件箱/收件箱模式，如*事务性事件的发件箱/收件箱*部分中所述。

如果您不配置发件箱/收件箱模式或使用`LocalDistributedEventBus`，则事件默认在工作单元结束时发布，就在工作单元完成之前（这意味着在事件处理程序中抛出异常仍会回滚工作单元），即使您在工作单元中间发布它们也是如此。如果您想立即发布事件，您可以在使用方法时设置`onUnitOfWorkComplete`为。`false``IDistributedEventBus.PublishAsync`

> 除非您没有特殊要求，否则建议保留默认行为。`onUnitOfWorkComplete`在实体/聚合根类内发布事件时选项不可用（请参阅*在实体/聚合根类内发布事件*部分）。

## 事务事件的发件箱/收件箱

这**[事务发件箱模式](https://microservices.io/patterns/data/transactional-outbox.html)**用于在**同一交易**操作应用程序数据库。启用 outbox 后，分布式事件将与您的数据更改一起保存到数据库中的同一事务中，然后由具有重试系统的单独[后台工作程序](https://abp.io/docs/latest/framework/infrastructure/background-workers)发送到实际的消息代理。这样，它可确保您的数据库状态与已发布的事件之间的一致性。

这**交易收件箱模式**另一方面，它首先将传入的事件保存到数据库中。然后（在[后台工作程序](https://abp.io/docs/latest/framework/infrastructure/background-workers)中）以事务方式执行事件处理程序，并在同一事务中从收件箱队列中删除事件。它通过保留已处理的消息一段时间并丢弃从消息代理收到的重复事件来确保事件仅执行一次。

启用事件发件箱和收件箱系统需要为您的应用程序执行一些手动步骤。请按照以下部分中的说明操作以使它们运行。

> 发件箱和收件箱可以单独启用和配置，因此如果您愿意，可以只使用其中一个。

### 先决条件

- 当您运行应用程序/服务的多个实例时，发件箱/收件箱系统使用分布式锁系统来处理并发。因此，您应该**配置分布式锁系统**与其中一个提供商合作，如[本文档中所述](https://abp.io/docs/latest/framework/infrastructure/distributed-locking)。
- 发件箱/收件箱系统支持[Entity Framework Core](https://abp.io/docs/latest/framework/data/entity-framework-core) (EF Core) 和[MongoDB](https://abp.io/docs/latest/framework/data/mongodb) **数据库提供程序**开箱即用。因此，您的应用程序应该使用这些数据库提供程序之一。对于其他数据库提供程序，请参阅“*实现自定义数据库提供程序”*部分。

> 如果您正在使用 MongoDB，请确保启用了 MongoDB 版本 4.0 中引入的多文档数据库事务。请参阅[MongoDB](https://abp.io/docs/latest/framework/data/mongodb)文档的*事务*部分。

### 启用事件发件箱

启用事件发件箱取决于您的数据库提供商。

#### 为 Entity Framework Core 启用事件发件箱

打开你的`DbContext`类，实现`IHasEventOutbox`接口。你最终应该`DbSet`在类中添加一个属性`DbContext`：

```C#
public DbSet<OutgoingEventRecord> OutgoingEvents { get; set; }
```

`OnModelCreating`在你的类的方法中添加以下几行`DbContext`：

```
builder.ConfigureEventOutbox();
```

使用标准`Add-Migration`和`Update-Database`命令将更改应用到您的数据库中。如果您想使用命令行终端，请在数据库集成项目的根目录中运行以下命令：

```
dotnet ef migrations add "Added_Event_Outbox"
dotnet ef database update
```

[最后，在模块类](https://abp.io/docs/latest/framework/architecture/modularity/basics)`ConfigureServices`的方法里面写入以下配置代码（替换为您自己的类）：`YourDbContext``DbContext`

```
Configure<AbpDistributedEventBusOptions>(options =>
{
    options.Outboxes.Configure(config =>
    {
        config.UseDbContext<YourDbContext>();
    });
});
```

#### 为 MongoDB 启用事件发件箱

打开你的`DbContext`类，实现`IHasEventOutbox`接口。你最终应该`IMongoCollection`在类中添加一个属性`DbContext`：

```C#
public IMongoCollection<OutgoingEventRecord> OutgoingEvents => Collection<OutgoingEventRecord>();
```



`CreateModel`在你的类的方法中添加以下几行`DbContext`：

```C#
modelBuilder.ConfigureEventOutbox();
```



[最后，在模块类](https://abp.io/docs/latest/framework/architecture/modularity/basics)`ConfigureServices`的方法里面写入以下配置代码（替换为您自己的类）：`YourDbContext``DbContext`

```C#
Configure<AbpDistributedEventBusOptions>(options =>
{
    options.Outboxes.Configure(config =>
    {
        config.UseMongoDbContext<MyProjectNameDbContext>();
    });
});
```



#### 发件箱分布式锁定

> **重要的**：发件箱发送服务使用分布式锁来确保只有一个应用程序实例同时使用发件箱队列。分布式锁定密钥在每个数据库中应该是唯一的。`config`对象（在`options.Outboxes.Configure(...)`方法中）具有一个`DatabaseName`属性，该属性用于分布式锁定密钥以确保唯一性。`DatabaseName`由方法自动设置，从类的属性`UseDbContext`中获取数据库名称。因此，如果您的系统中存在多个数据库，请确保对同一个数据库使用相同的连接字符串名称，但对不同的数据库使用不同的连接字符串名称。如果无法确保这一点，您可以手动设置（在行后）以确保唯一性。`ConnectionStringName``YourDbContext``config.DatabaseName``UseDbContext`

### 启用事件收件箱

启用事件收件箱取决于您的数据库提供商。

#### 为 Entity Framework Core 启用事件收件箱

打开你的`DbContext`类，实现`IHasEventInbox`接口。你最终应该`DbSet`在类中添加一个属性`DbContext`：

```C#
public DbSet<IncomingEventRecord> IncomingEvents { get; set; }
```



`OnModelCreating`在你的类的方法中添加以下几行`DbContext`：

```C#
builder.ConfigureEventInbox();
```



使用标准`Add-Migration`和`Update-Database`命令将更改应用到您的数据库中。如果您想使用命令行终端，请在数据库集成项目的根目录中运行以下命令：

```bash
dotnet ef migrations add "Added_Event_Inbox"
dotnet ef database update
```



[最后，在模块类](https://abp.io/docs/latest/framework/architecture/modularity/basics)`ConfigureServices`的方法里面写入以下配置代码（替换为您自己的类）：`YourDbContext``DbContext`

```C#
Configure<AbpDistributedEventBusOptions>(options =>
{
    options.Inboxes.Configure(config =>
    {
        config.UseDbContext<YourDbContext>();
    });
});
```



#### 为 MongoDB 启用事件收件箱

打开你的`DbContext`类，实现`IHasEventInbox`接口。你最终应该`IMongoCollection`在类中添加一个属性`DbContext`：

```C#
public IMongoCollection<IncomingEventRecord> IncomingEvents => Collection<IncomingEventRecord>();
```



`CreateModel`在你的类的方法中添加以下几行`DbContext`：

```C#
modelBuilder.ConfigureEventInbox();
```



[最后，在模块类](https://abp.io/docs/latest/framework/architecture/modularity/basics)`ConfigureServices`的方法里面写入以下配置代码（替换为您自己的类）：`YourDbContext``DbContext`

```C#
Configure<AbpDistributedEventBusOptions>(options =>
{
    options.Inboxes.Configure(config =>
    {
        config.UseMongoDbContext<MyProjectNameDbContext>();
    });
});
```



#### 收件箱的分布式锁定

> **重要的**：收件箱处理服务使用分布式锁来确保只有一个应用程序实例同时使用收件箱队列。分布式锁定密钥在每个数据库中应该是唯一的。`config`对象（在`options.Inboxes.Configure(...)`方法中）具有一个`DatabaseName`属性，该属性用于分布式锁定密钥以确保唯一性。`DatabaseName`由方法自动设置，从类的属性`UseDbContext`中获取数据库名称。因此，如果您的系统中存在多个数据库，请确保对同一个数据库使用相同的连接字符串名称，但对不同的数据库使用不同的连接字符串名称。如果无法确保这一点，您可以手动设置（在行后）以确保唯一性。`ConnectionStringName``YourDbContext``config.DatabaseName``UseDbContext`

### 附加配置

> 默认配置对于大多数情况来说已经足够了。但是，你可能需要为发件箱和收件箱设置一些选项。

#### 发件箱配置

记住发件箱是如何配置的：

```C#
Configure<AbpDistributedEventBusOptions>(options =>
{
    options.Outboxes.Configure(config =>
    {
        // TODO: Set options
    });
});
```



此处，对象具有以下属性`config`：

- `IsSendingEnabled`（默认值`true`：）：您可以设置`false`为禁用将发件箱事件发送到实际事件总线。如果禁用此功能，事件仍可以添加到发件箱，但不会发送。如果您有多个应用程序（或应用程序实例）写入发件箱，但使用其中一个发送事件，这可能会有所帮助。
- `Selector`：用于过滤此配置所用事件 (ETO) 类型的谓词。应返回`true`以选择事件。默认情况下，它会选择所有事件。如果您想忽略发件箱中的某些 ETO 类型，或者想要定义命名的发件箱配置并在这些配置中分组事件，这将特别有用。请参阅*命名配置*部分。
- `ImplementationType`：实现发件箱数据库操作的类的类型。通常在调用时设置，`UseDbContext`如前所示。有关高级用法，请参阅*实现自定义发件箱/收件箱数据库提供程序部分。*
- `DatabaseName`：用于此发件箱配置的数据库的唯一数据库名称。请参阅**重要的***启用事件发件箱/收件箱*部分末尾的段落 。

#### 收件箱配置

记住收件箱是如何配置的：

```C#
Configure<AbpDistributedEventBusOptions>(options =>
{
    options.Inboxes.Configure(config =>
    {
        // TODO: Set options
    });
});
```



此处，对象具有以下属性`config`：

- `IsProcessingEnabled`（默认值`true`：）：您可以设置`false`为禁用收件箱中的事件处理。如果禁用此功能，仍然可以接收事件，但不会执行。如果您有多个应用程序（或应用程序实例），但使用其中一个来执行事件处理程序，这会很有用。
- `EventSelector`：用于过滤此配置所用事件 (ETO) 类型的谓词。如果您想要忽略收件箱中的某些 ETO 类型，或者想要定义命名收件箱配置并在这些配置中分组事件，则此谓词特别有用。请参阅*命名配置*部分。
- `HandlerSelector`：用于过滤此配置所用的事件处理类型（实现`IDistributedEventHandler<TEvent>`接口的类）的谓词。如果您想要忽略收件箱处理中的某些事件处理程序类型，或者想要定义命名的收件箱配置并在这些配置中分组事件处理程序，则此谓词特别有用。请参阅*命名配置*部分。
- `ImplementationType`：实现收件箱数据库操作的类的类型。通常在调用时设置，`UseDbContext`如前所示。有关高级用法，请参阅*实现自定义发件箱/收件箱数据库提供程序部分。*
- `DatabaseName`：用于此发件箱配置的数据库的唯一数据库名称。请参阅**重要的***启用事件收件箱*部分末尾的段落。

#### AbpEventBusBoxes选项

`AbpEventBusBoxesOptions`可用于微调收件箱和发件箱系统的工作方式。对于大多数系统来说，使用默认设置就足够了，但您可以根据需要对其进行配置以优化系统。

[就像所有选项类](https://abp.io/docs/latest/framework/fundamentals/options)一样，`AbpEventBusBoxesOptions`可以在[模块类](https://abp.io/docs/latest/framework/architecture/modularity/basics)`ConfigureServices`的方法中进行配置，如下面的代码块所示：

```C#
Configure<AbpEventBusBoxesOptions>(options =>
{
    // TODO: configure the options
});
```



`AbpEventBusBoxesOptions`有以下属性需要配置：

- `BatchPublishOutboxEvents`：可用于启用或禁用批量发布事件到消息代理。如果分布式事件总线提供商支持批量发布，则批量发布可以正常工作。如果不支持，则事件将作为后备逻辑逐个发送。请将其保持为启用状态，因为它尽可能地提高性能。默认值为`true`（启用）。
- `PeriodTimeSpan`：收件箱和发件箱消息处理器检查数据库中是否有新事件的周期。默认值为 2 秒（`TimeSpan.FromSeconds(2)`）。
- `CleanOldEventTimeIntervalSpan`：事件收件箱系统会定期检查并从数据库中的收件箱中删除旧的已处理事件。您可以设置此值来确定检查周期。默认值为 6 小时（`TimeSpan.FromHours(6)`）。
- `WaitTimeToDeleteProcessedInboxEvents`：收件箱事件即使成功处理，一段时间内也不会从数据库中删除。这是为了让系统防止对同一事件进行多次处理（如果事件代理发送两次）。此配置值决定保留已处理事件的时间。默认值为 2 小时（`TimeSpan.FromHours(2)`）。
- `InboxWaitingEventMaxCount`：一次从数据库收件箱中查询的最大事件数。默认值为 1000。
- `OutboxWaitingEventMaxCount`：一次从数据库发件箱中查询的最大事件数。默认值为 1000。
- `DistributedLockWaitDuration`：当运行同一应用程序的多个实例时，ABP 使用[分布式锁定](https://abp.io/docs/latest/framework/infrastructure/distributed-locking)来防止同时访问数据库中的收件箱和发件箱消息。如果应用程序的实例无法获取锁定，它会在一段时间后尝试。这是该持续时间的配置。默认值为 15 秒（`TimeSpan.FromSeconds(15)`）。

### 跳过发件箱

`IDistributedEventBus.PublishAsync`方法提供了一个可选参数`useOutbox`，默认设置为`true`。如果您绕过发件箱并立即发布事件，则可以将其设置为，`false`以进行特定的事件发布操作。

### 高级主题

#### 命名配置

> 本节中解释的所有概念也适用于收件箱配置。我们将仅显示发件箱的示例，以使文档更简短。

参见以下发件箱配置代码：

```C#
Configure<AbpDistributedEventBusOptions>(options =>
{
    options.Outboxes.Configure(config =>
    {
        //TODO
    });
});
```



这等效于以下代码：

```C#
Configure<AbpDistributedEventBusOptions>(options =>
{
    options.Outboxes.Configure("Default", config =>
    {
        //TODO
    });
});
```



`Default`此代码表示配置名称。如果您未指定它（如在上一个代码块中），`Default`则将其用作配置名称。

这意味着您可以为发件箱（也为收件箱）定义多个具有不同名称的配置。ABP 运行所有配置的发件箱。

如果您的应用程序有多个数据库，并且您想要为不同的数据库运行不同的发件箱队列，则可能需要多个发件箱。在这种情况下，您可以使用该`Selector`选项来决定事件应由发件箱处理。请参阅上面的*其他配置*部分。

#### 实现自定义发件箱/收件箱数据库提供程序

如果您的应用程序或服务使用[EF Core](https://abp.io/docs/latest/framework/data/entity-framework-core)和[MongoDB](https://abp.io/docs/latest/framework/data/mongodb)以外的数据库提供程序，则应手动将发件箱/收件箱系统与您的数据库提供程序集成。

> 发件箱和收件箱表/数据必须与应用程序数据存储在同一个数据库中（因为我们想要创建一个包含应用程序数据库操作和发件箱/收件箱表操作的单一数据库事务）。否则，您应该关注大多数供应商不提供的分布式（多数据库）事务支持，可能需要额外的配置。

ABP 提供了`IEventOutbox`和`IEventInbox`抽象作为发件箱/收件箱系统的扩展点。你可以通过实现这些接口来创建类，并将它们注册到[依赖注入中](https://abp.io/docs/latest/framework/fundamentals/dependency-injection)。

一旦您实现了自定义事件框，您就可以配置`AbpDistributedEventBusOptions`使用事件框类：

```C#
Configure<AbpDistributedEventBusOptions>(options =>
{
    options.Outboxes.Configure(config =>
    {
        config.ImplementationType = typeof(MyOutbox); //Your Outbox class
    });
    
    options.Inboxes.Configure(config =>
    {
        config.ImplementationType = typeof(MyInbox); //Your Inbox class
    });
});
```



##  参考：

https://abp.io/docs/latest/framework/infrastructure/event-bus/distributed
