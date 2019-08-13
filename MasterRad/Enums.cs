using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad
{
    public enum ExceptionLogMethod
    {
        JsonSerialize = 1,
        ToString = 2
    }

    public enum TestStatus
    {
        Scheduled = 1,
        InProgress = 2,
        Completed = 3
    }
}
