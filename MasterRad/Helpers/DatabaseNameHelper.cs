using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Helpers
{
    public static class DatabaseNameHelper
    {
        public static IEnumerable<KeyValuePair<int, string>> SynthesisTestExam(IEnumerable<int> studentIds, int testId)
        {
            return studentIds.Select(sid => new KeyValuePair<int, string>(sid, $"ST_{testId}_{sid}"));
        }
    }
}
