using Default_Project.Cores.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace Default_Project.Services
{
    public class CacheService : ICache
    {
        private readonly IDatabase _database;

        public CacheService(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }
        public async Task<TimeSpan> CacheDataAsync(string key, object value, TimeSpan ExpireTime = default)
        {
            if (ExpireTime == default)
                ExpireTime = TimeSpan.FromMinutes(9); // Default expiration time

            if (value is null) return ExpireTime;
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            var serializedValue = JsonSerializer.Serialize(value, options);
            await _database.StringSetAsync(key, serializedValue, ExpireTime);
            return ExpireTime;
        }
        
        public async Task<string?> GetDataAsync(string key)
        {
            var res = await _database.StringGetAsync(key);
            return res.IsNullOrEmpty ? null : res.ToString();
        }
    }
}
