using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Models.ViewModels
{
    public class SynthesisExamVM
    {
        public int TestId { get; set; }
        public int StudentId { get; set; }
        public string TimeStamp { get; set; }
        public string NameOnServer { get; set; }

        public string SqlScript { get; set; }
        public string ModelDescription { get; set; }
        public string TaskDescription { get; set; }
    }
}
