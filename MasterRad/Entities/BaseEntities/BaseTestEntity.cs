using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Entities
{
    public abstract class BaseTestEntity : BaseEntity<int>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        [Required, MaxLength(255)]
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
