using MasterRad;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Entities
{
    public class ExceptionLogEntity : BaseEntityInsertOnly<int>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        public string Exception { get; set; }

        public ExceptionLogMethod LogMethod { get; set; }
        public string SerializeError { get; set; }
    }
}
