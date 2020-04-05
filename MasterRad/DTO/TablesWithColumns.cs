using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.DTO
{
    public class TableWithColumns
    {
        public string TableFullName { get; set; }
        public IEnumerable<string> ColumnNames { get; set; }
    }
}
