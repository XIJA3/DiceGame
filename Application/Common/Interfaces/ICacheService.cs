using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTemplate.Server.Common.Interfaces
{
    public interface ICacheService
    {
        public Task SetDataAsync<T>(string key, T data, TimeSpan? absoluteExpireTime = null, TimeSpan? unusedExpireTime = null);
        public Task<T> GetDataAsync<T>(string key);
        public Task RemoveDataAsync(string key);
    }
}
