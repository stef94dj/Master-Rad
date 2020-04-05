using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.DTO.RQ
{
    public class AnswerSynthesisRQ
    {
        public int TestId { get; set; }
        public int StudentId { get; set; }
        public byte[] TimeStamp { get; set; }
        public string SqlScript { get; set; }
    }
}
