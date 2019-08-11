using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasterRad.Entities
{
    public class SynthesisTestEntity : BaseTestEntity
    {
        public int TaskId { get; set; }
        public TaskEntity Task { get; set; }

        public List<SynthesisTestStudentEntity> SynthesisTestStudents { get; set; }
    }
}
