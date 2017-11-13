using Microsoft.Extensions.Configuration;
using YD.Data.Utils;
using YD.Foundation.Config;

namespace MEC.WebApi.Utils
{
    public class AfterStartup
    {
        private readonly RedisHeartbeat redisHeartbeat;
        private readonly CacheLoader cacheLoader;

        public AfterStartup(IConfigurationRoot theConfig, RedisHeartbeat theHeartbeat, CacheLoader theCacheLoader)
        {
            CacheSettings.SetConfig(theConfig);
            this.redisHeartbeat = theHeartbeat;
            this.cacheLoader = theCacheLoader;
        }

        public void Start()
        {
            redisHeartbeat.Start();
            cacheLoader.Start();
        }
    }
}
