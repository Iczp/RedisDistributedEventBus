using Volo.Abp.Modularity;

namespace IczpNet.RedisDistributedEventBus;

/* Inherit from this class for your application layer tests.
 * See SampleAppService_Tests for example.
 */
public abstract class RedisDistributedEventBusApplicationTestBase<TStartupModule> : RedisDistributedEventBusTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
