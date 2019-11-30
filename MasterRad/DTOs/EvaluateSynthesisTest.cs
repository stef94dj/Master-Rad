using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.DTOs
{
    public class EvaluateSynthesisTest
    {
        public int SynthesisTestId { get; set; }
        public int StudentId { get; set; }
        public bool EvaluateWithSecretData { get; set; }
    }
}
