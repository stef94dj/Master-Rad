using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.DTO
{
    public class ColumnInfoDTO
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsNullable { get; set; }
        public string DefaultValue { get; set; }
        public int? MaxLength { get; set; }
    }
}
