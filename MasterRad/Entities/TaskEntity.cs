using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Entities
{
    public class TaskEntity : BaseEntity<int>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; }
        public string Description { get; set; }

        public int DbTemplateId { get; set; }
        [ForeignKey("DbTemplateId")]
        public DbTemplateEntity Template { get; set; }

        public bool IsDataSet { get; set; }
        public string NameOnServer { get; set; }
    }
}
