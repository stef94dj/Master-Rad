using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.DTO.RQ
{
    public class QueryExecuteRQ
    {
        public string DatabaseName { get; set; }
        public string SQLQuery { get; set; }
    }  
}
