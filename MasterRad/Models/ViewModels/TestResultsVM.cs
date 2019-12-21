using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Models.ViewModels
{
    public class TestResultsVM
    {
        public int TestId { get; set; }
        public TestType TestType { get; set; }
        public int JobId { get; set; }
    }
}
