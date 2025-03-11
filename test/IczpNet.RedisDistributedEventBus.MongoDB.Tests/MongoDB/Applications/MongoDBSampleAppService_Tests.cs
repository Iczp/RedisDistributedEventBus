using IczpNet.RedisDistributedEventBus.MongoDB;
using IczpNet.RedisDistributedEventBus.Samples;
using Xunit;

namespace IczpNet.RedisDistributedEventBus.MongoDb.Applications;

[Collection(MongoTestCollection.Name)]
public class MongoDBSampleAppService_Tests : SampleAppService_Tests<RedisDistributedEventBusMongoDbTestModule>
{

}
