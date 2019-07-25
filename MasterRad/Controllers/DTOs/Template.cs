using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Table = System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, object>>;

namespace MasterRad.DTOs
{
    public class DatabaseCreateRQ
    {
        public string Name { get; set; }
    }
    public class DatabaseCreateRS { }

    public class UpdateDescriptionRQ : UpdateDTO
    {
        public string Description { get; set; }
    }
    public class UpdateDescriptionRS { }

    public class UpdateNameRQ : UpdateDTO
    {
        public string Name { get; set; }
    }
    public class UpdateNameRS { }

    public class SetSqlScriptRQ : UpdateDTO
    {
        public string DbName { get; set; }
        public string SqlScript { get; set; }
    }
    public class SetSqlScriptRS { }
}
