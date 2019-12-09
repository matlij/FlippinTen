using Akavache;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FlippinTen.Core.Services
{
    public class BaseService
    {

        public BaseService(IBlobCache cache)
        {
            Cache = cache ?? BlobCache.LocalMachine;
        }

        protected IBlobCache Cache { get; }

        public async Task<T> GetFromCache<T>(string key)
        {
            try
            {
                return await Cache.GetObject<T>(key);
            }
            catch (KeyNotFoundException)
            {
                Debug.WriteLine(key + " not found in cache.");
                return default(T);
            }
        }
    }
}
