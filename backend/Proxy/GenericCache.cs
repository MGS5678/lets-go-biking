using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace Proxy
{
    public class GenericCache<T> where T : new()

    {
        private Dictionary<string, T> _cache;
        private DateTimeOffset dt_default;
        private Dictionary<string, DateTimeOffset> _ttl;
        public GenericCache()
        {
            _cache = new Dictionary<string, T>();
            dt_default = ObjectCache.InfiniteAbsoluteExpiration;
            _ttl = new Dictionary<string, DateTimeOffset>();
        }

        public T Get(string CacheItemName)
        {
            if (_cache.ContainsKey(CacheItemName) && _cache[CacheItemName] != null)
            {
                if (DateTimeOffset.Now <= _ttl[CacheItemName])
                {
                    return _cache[CacheItemName];
                }

                T data = new T();
                _cache[CacheItemName] = data;
                _ttl[CacheItemName] = dt_default;
                return data;
            }
            else
            {
                T data = new T();
                _cache[CacheItemName] = data;
                _ttl[CacheItemName] = dt_default;
                return data;
            }
        }

        public T Get(string CacheItemName, double dt_seconds)
        {
            if (_cache.ContainsKey(CacheItemName) && _cache[CacheItemName] != null)
            {
                if (DateTimeOffset.Now <= _ttl[CacheItemName])
                {
                    return _cache[CacheItemName];
                }

                T data = new T();
                _cache[CacheItemName] = data;
                _ttl[CacheItemName] = DateTimeOffset.Now.AddSeconds(dt_seconds);
                return data;
            }
            else
            {
                T data = new T();
                _cache[CacheItemName] = data;
                _ttl[CacheItemName] = DateTimeOffset.Now.AddSeconds(dt_seconds);
                return data;
            }
        }

        public T Get(string CacheItemName, DateTimeOffset dt)
        {
            if (_cache.ContainsKey(CacheItemName) && _cache[CacheItemName] != null)
            {
                if (DateTimeOffset.Now <= _ttl[CacheItemName])
                {
                    return _cache[CacheItemName];
                }

                T data = new T();
                _cache[CacheItemName] = data;
                _ttl[CacheItemName] = dt;
                return data;
            }
            else
            {
                T data = new T();
                _cache[CacheItemName] = data;
                _ttl[CacheItemName] = dt;
                return data;
            }
        }

        /*
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
        }*/
    }
}
