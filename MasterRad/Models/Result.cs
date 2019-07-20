using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Models
{
    public class Result <T>
    {
        public T Value { get; set; }
        public List<string> Errors { get; set; }

        public static Result<T> Success(T result)
        {
            return new Result<T>() {
                Value = result,
                Errors = null
            };
        }

        public static Result<T> Fail(List<string> errors)
        {
            return new Result<T>()
            {
                Value = default(T),
                Errors = errors
            };
        }
    }
}
