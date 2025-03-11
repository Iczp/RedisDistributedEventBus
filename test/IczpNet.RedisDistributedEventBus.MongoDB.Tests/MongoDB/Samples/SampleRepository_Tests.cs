using IczpNet.RedisDistributedEventBus.Samples;
using Xunit;

namespace IczpNet.RedisDistributedEventBus.MongoDB.Samples;

[Collection(MongoTestCollection.Name)]
public class SampleRepository_Tests : SampleRepository_Tests<RedisDistributedEventBusMongoDbTestModule>
{
    /* Don't write custom repository tests here, instead write to
     * the base class.
     * One exception can be some specific tests related to MongoDB.
     */
}
