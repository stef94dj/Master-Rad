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

    public abstract class DataBaseRQ
    {
        public string DatabaseName { get; set; }
        public string TableName { get; set; }
    }

    public class DataCreateRQ : DataBaseRQ
    {
        public List<Cell> ValuesNew { get; set; }
    }

    public class DataDeleteRQ : DataBaseRQ
    {
        public List<Cell> Values { get; set; }
    }

    public class DataUpdateRQ : DataCreateRQ
    {
        public List<Cell> ValuesPrevious { get; set; }
    }
}
