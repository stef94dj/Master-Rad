using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Models.ViewModels
{
    public class AnalysisExamVM
    {
        public string Title { get; set; }
        public string ModelDescription { get; set; }
        public string TaskDescription { get; set; }
        public string SqlSolutionForEvaluation { get; set; }
        public ModifyDatabasePartialVM FailingInputVM { get; set; }
        public ModifyTablePartialVM StudentOutputVM { get; set; }
        public ModifyTablePartialVM CorrectOutputVM { get; set; }
    }
}
