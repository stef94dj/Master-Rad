using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Entities
{
    public class AnalysisPaperEntity : BaseEntity<int>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        //EVALUATION DATA

        public int ATS_AnalysisTestId { get; set; }
        public int ATS_StudentId { get; set; }

        [ForeignKey("ATS_StudentId,ATS_AnalysisTestId")]
        public AnalysisTestStudentEntity AnalysisTestStudent { get; set; }
    }
}
