using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;

namespace WMK_BE_RecipesAndPlans_DataAccess.Repository.Implement
{
    public class RedisService : IRedisService
    {
        private readonly IConnectionMultiplexer _redis;
        public RedisService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }
        public async Task<T> GetValueAsync<T>(string key)
        {
            var db = _redis.GetDatabase();
            var json = await db.StringGetAsync(key);
            return json.HasValue ? JsonConvert.DeserializeObject<T>(json) : default;
        }

        public Task<bool> RemoveAsync(string key)
        {
            var db = _redis.GetDatabase();
            return db.KeyDeleteAsync(key);
        }

        public async  Task<bool> SetValueAsync<T>(string key, T value, TimeSpan timeOut)
        {
            var db = _redis.GetDatabase();
            var json = JsonConvert.SerializeObject(value, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
            var dataRs= await db.StringSetAsync(key, json, timeOut);
            return dataRs;
          
        }
    }
}
