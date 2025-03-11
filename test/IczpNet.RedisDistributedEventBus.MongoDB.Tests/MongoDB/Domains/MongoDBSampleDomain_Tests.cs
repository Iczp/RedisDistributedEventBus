using IczpNet.RedisDistributedEventBus.Samples;
using Xunit;

namespace IczpNet.RedisDistributedEventBus.MongoDB.Domains;

[Collection(MongoTestCollection.Name)]
public class MongoDBSampleDomain_Tests : SampleManager_Tests<RedisDistributedEventBusMongoDbTestModule>
{

}
