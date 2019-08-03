using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DiplomskiRad_ds130211d.Models
{
    public class DatabaseViewModel
    {
        public DatabaseViewModel()
        {
            Tables = new List<Table>();
        }
        public string Name { get; set; }
        public List<Table> Tables { get; set; }

        public class Table
        {
            public Table()
            {
                NamesOfCollumns = new List<string>();
                Rows = new List<List<string>>();
            }
            public string Name { get; set; }
            public List<string> NamesOfCollumns { get; set; }
            public List<List<string>> Rows { get; set; }

        }
    }
}