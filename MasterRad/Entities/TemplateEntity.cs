using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Entities
{
    public class TemplateEntity : BaseEntity<int>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(8191)]
        public string ModelDescription { get; set; }
        [MaxLength(255)]
        public string NameOnServer { get; set; }

        public bool IsPublic { get; set; }

        public List<TaskEntity> Tasks{ get; set; }
    }
}
