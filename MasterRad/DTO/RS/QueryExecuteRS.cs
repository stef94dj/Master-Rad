using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MasterRad.DTO.RS
{
    public class QueryExecuteRS
    {
        public QueryExecuteRS(List<string> messages, List<TableDTO> tables, int? rowsAffected)
        {
            Messages = messages;
            Tables = tables;
            RowsAffected = rowsAffected;
        }
        public List<string> Messages { get; set; }
        public int? RowsAffected { get; set; }
        public List<TableDTO> Tables { get; set; }

        [JsonIgnore]
        public bool NoMessages
        {
            get
            {
                return Messages == null || !Messages.Any();
            }
        }
    }
}
