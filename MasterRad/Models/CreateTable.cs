using MasterRad.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Models
{
    public class CreateTable
    {
        public string TableName { get; set; }
        public IEnumerable<Column> Columns { get; set; }
    }
}
