using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Entities
{
    public class UnhandledExceptionLogEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }

        public string Exception { get; set; }

        public string Body { get; set; }
        public string Headers { get; set; }
        public string Cookies { get; set; }
        public string Path { get; set; }
        public string PathBase { get; set; }
        public string Method { get; set; }
        public string Protocol { get; set; }
        public string QueryString { get; set; }
        public string Query { get; set; }

    }
}
