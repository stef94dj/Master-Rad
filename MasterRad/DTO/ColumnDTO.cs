using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.DTO
{
    public class ColumnDTO
    {
        public ColumnDTO(string name, string sqlType)
        {
            Name = name;
            SqlType = sqlType;
        }

        public string Name { get; set; }
        public string SqlType { get; set; }
        public bool? IsPrimaryKey { get; set; }
    }
}
