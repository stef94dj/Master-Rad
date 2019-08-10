using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Entities
{
    public class SolutionColumnEntity : BaseEntityInsertOnly<int>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        public int TaskId { get; set; }
        [ForeignKey("TaskId")]
        public TaskEntity Task { get; set; }

        [MaxLength(255), Required]
        public string ColumnName { get; set; }
    }
}
