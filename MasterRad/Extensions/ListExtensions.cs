using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Extensions
{
    public static class ListExtensions
    {
        public static IEnumerable<IEnumerable<T>> ToChunks<T>(this IEnumerable<T> collection, int maxSize)
        {
            var res = new List<List<T>>();
            var chunk = new List<T>();
            var i = 0;
            foreach (var item in collection)
            {
                chunk.Add(item);

                if (chunk.Count() == maxSize || i + 1 == collection.Count())
                {
                    res.Add(chunk);
                    chunk = new List<T>();
                }

                i++;
            }
            return res;
        }
    }
}
