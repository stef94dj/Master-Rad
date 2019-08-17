using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.DTOs
{
    public class AnswerSynthesisRQ
    {
        public int TestId { get; set; }
        public int SynthesisPaperId { get; set; }
        public byte[] SynthesisPaperTimeStamp { get; set; }
        public string SqlScript { get; set; }
    }
}
