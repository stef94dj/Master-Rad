using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasterRad.Entities
{
    public class AnalysisEvaluationEntity : BaseEntity<int>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        public int ATS_AnalysisTestId { get; set; }
        public int ATS_StudentId { get; set; }
        [ForeignKey("ATS_StudentId,ATS_AnalysisTestId")]
        public AnalysisTestStudentEntity AnalysisTestStudent { get; set; }

        public EvaluationProgress Progress { get; set; }
        public string Message { get; set; }
        public AnalysisEvaluationType Type { get; set; }
    }
}
