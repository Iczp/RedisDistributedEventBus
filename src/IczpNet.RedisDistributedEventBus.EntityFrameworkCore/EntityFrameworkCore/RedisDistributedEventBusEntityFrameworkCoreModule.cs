using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace IczpNet.RedisDistributedEventBus.EntityFrameworkCore;

[DependsOn(
    typeof(RedisDistributedEventBusDomainModule),
    typeof(AbpEntityFrameworkCoreModule)
)]
public class RedisDistributedEventBusEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<RedisDistributedEventBusDbContext>(options =>
        {
                /* Add custom repositories here. Example:
                 * options.AddRepository<Question, EfCoreQuestionRepository>();
                 */
        });
    }
}
