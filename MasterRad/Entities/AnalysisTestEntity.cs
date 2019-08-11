using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasterRad.Entities
{
    public class AnalysisTestEntity : BaseEntity<int>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        public bool IsActive { get; set; }

        public int SynthesisPaperId { get; set; }
        public SynthesisPaperEntity SynthesisPaper { get; set; }

        public List<AnalysisTestStudentEntity> AnalysisTestStudents{ get; set; }
    }
}
