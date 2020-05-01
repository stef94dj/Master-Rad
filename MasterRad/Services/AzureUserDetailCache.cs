using MasterRad.DTO;
using MasterRad.Models.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Services
{
    public interface IAzureUserDetailCache
    {
        List<AzureUserDTO> Get(IEnumerable<Guid> keys, out IEnumerable<Guid> notFound);
        void Add(IEnumerable<AzureUserDTO> entries);
    }

    public class AzureUserDetailCache : IAzureUserDetailCache
    {
        private MemoryCache _cache { get; set; }
        private readonly MemoryCacheEntryOptions _entryOptions;

        public AzureUserDetailCache(IOptions<UserDetailCache> cacheConfig)
        {
            var config = cacheConfig.Value;

            _cache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = config.TotalCacheSize
            });

            _entryOptions = new MemoryCacheEntryOptions()
                               .SetSize(config.CacheItemSize)
                               .SetSlidingExpiration(TimeSpan.FromDays(config.TimeoutSlidingDays))
                               .SetAbsoluteExpiration(TimeSpan.FromDays(config.TimeoutMaxDays));
        }

        public List<AzureUserDTO> Get(IEnumerable<Guid> keys, out IEnumerable<Guid> notFound)
        {
            var res = new List<AzureUserDTO>();
            var notFoundIds = new List<Guid>();

            foreach (var key in keys)
            {
                if (_cache.TryGetValue(key, out AzureUserDTO entry))
                    res.Add(entry);
                else
                    notFoundIds.Add(key);
            }

            notFound = notFoundIds;
            return res;
        }

        public void Add(IEnumerable<AzureUserDTO> entries)
        {
            foreach (var entry in entries)
            {
                _cache.Set(entry.MicrosoftId, entry, _entryOptions);
            }
        }
    }
}
