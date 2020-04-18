using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToJsonString(this DateTime? dateTime)
        {
            return dateTime.HasValue ? dateTime.Value.ToString(Constants.JSDateFormat) : null;
        }
    }
}
