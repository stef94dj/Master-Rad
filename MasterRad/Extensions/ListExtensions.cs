using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Extensions
{
    public static class ListExtensions
    {
        public static List<List<T>> ToChunks<T>(this List<T> collection, int maxSize)
        {
            var res = new List<List<T>>();
            var chunk = new List<T>();
            for (int i = 0; i < collection.Count(); i++)
            {
                chunk.Add(collection[i]);

                if (chunk.Count() == maxSize || i + 1 == collection.Count())
                {
                    res.Add(chunk);
                    chunk = new List<T>();
                }
            }
            return res;
        }
    }
}
