using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.DTOs
{
    public class Cell
    {
        public string ColumnName { get; set; }
        public string Value { get; set; }
    }

    public class DataInsertRQ
    {
        public string DatabaseName { get; set; }
        public string TableName { get; set; }
        public List<Cell> DataRecord { get; set; }
    }

    public class DataInsertRS
    {
        
    }
}
