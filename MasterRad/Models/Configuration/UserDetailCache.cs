using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Models.Configuration
{
    public class UserDetailCache
    {
        public int TotalCacheSize { get; set; }
        public int CacheItemSize { get; set; }
        public int TimeoutSlidingDays { get; set; }
        public int TimeoutMaxDays { get; set; }
    }
}
