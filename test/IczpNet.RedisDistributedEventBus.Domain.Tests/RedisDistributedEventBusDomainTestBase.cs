using Volo.Abp.Modularity;

namespace IczpNet.RedisDistributedEventBus;

/* Inherit from this class for your domain layer tests.
 * See SampleManager_Tests for example.
 */
public abstract class RedisDistributedEventBusDomainTestBase<TStartupModule> : RedisDistributedEventBusTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
