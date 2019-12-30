using MasterRad;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Entities
{
    public class RequestLogEntity : BaseEntity<int>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        public string Body { get; set; }
        public string Headers { get; set; }
        public string Cookies { get; set; }
        public string Path { get; set; }
        public string PathBase { get; set; }
        public string Method { get; set; }
        public string Protocol { get; set; }
        public string QueryString { get; set; }
        public string Query { get; set; }

        public int ExceptionLogId { get; set; }
        [ForeignKey("ExceptionLogId")]
        public ExceptionLogEntity Exception { get; set; }
    }
}
