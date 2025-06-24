using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Default_Project.Cores.Interfaces
{
    public interface ICache
    {
        Task<TimeSpan> CacheDataAsync(string key, object value, TimeSpan ExpireTime);
        Task<string?> GetDataAsync(string key);
    }
}
