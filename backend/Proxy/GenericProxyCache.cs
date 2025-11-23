using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace Proxy
{
    public class GenericProxyCache

    {
        private Dictionary<string, object> _cache;
        private DateTimeOffset dt_default;
        private Dictionary<string, DateTimeOffset> _ttl;
        public GenericProxyCache()
        {
            _cache = new Dictionary<string, object>();
            dt_default = ObjectCache.InfiniteAbsoluteExpiration;
            _ttl = new Dictionary<string, DateTimeOffset>();
        }
        public object Get(string CacheItemName)
        {
            if (_cache.ContainsKey(CacheItemName))
            {
                if (DateTimeOffset.Now <= _ttl[CacheItemName])
                {
                    return _cache[CacheItemName];
                }
                else
                {
                    _cache.Remove(CacheItemName);
                    _ttl.Remove(CacheItemName);
                    return default;
                }
            }
            else
            {
                return default;
            }
        }
        public void Set(string CachItemName, object value)
        {
            _cache[CachItemName] = value;
            _ttl[CachItemName] = dt_default;
        }
        public void Set(string CachItemName, object value, DateTimeOffset absoluteExpiration)
        {
            _cache[CachItemName] = value;
            _ttl[CachItemName] = absoluteExpiration;
        }
    }
}
