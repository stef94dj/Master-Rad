using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Entities
{
    public class SynthesisPaperStudentEntity
    {
        public int StudentId { get; set; }
        public StudentEntity Student { get; set; }

        public int SynthesisPaperId { get; set; }
        public SynthesisPaperEntity SynthesisPaper{ get; set; }
        
        public AnalysisPaperEntity AnalysisPaper { get; set; }
    }
}
