using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Entities
{
    public class DbTemplateEntity : BaseEntity<int>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        public string DbName { get; set; }
        public string SqlScript { get; set; }
    }
}
