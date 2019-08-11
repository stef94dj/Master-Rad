using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasterRad.Entities
{
    public class SynthesisTestEntity : BaseEntity<int>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        public bool IsActive { get; set; }

        public int TaskId { get; set; }
        public TaskEntity Task { get; set; }

        public List<SynthesisTestStudentEntity> SynthesisTestStudents { get; set; }
    }
}
