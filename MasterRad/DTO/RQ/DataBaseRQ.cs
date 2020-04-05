using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.DTO.RQ
{
    public abstract class DataBaseRQ
    {
        public string DatabaseName { get; set; }
        public string TableName { get; set; }
        public string SchemaName { get; set; }
    }
}
