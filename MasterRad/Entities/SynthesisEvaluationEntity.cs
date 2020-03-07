using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasterRad.Entities
{
    public class SynthesisEvaluationEntity: BaseEntity<int>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        public int STS_SynthesisTestId { get; set; }
        public int STS_StudentId { get; set; }
        [ForeignKey("STS_StudentId,STS_SynthesisTestId")]
        public SynthesisTestStudentEntity SynthesisTestStudent { get; set; }

        public EvaluationProgress Progress { get; set; }
        public string Message { get; set; }
        public bool IsSecretDataUsed { get; set; }
    }
}
