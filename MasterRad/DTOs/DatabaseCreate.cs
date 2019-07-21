using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Table = System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, object>>;

namespace MasterRad.DTOs
{
    public class DatabaseCreateRQ
    {
        public string SQLScript { get; set; }
    }

    public class DatabaseCreateRS
    {
      
    }
}
