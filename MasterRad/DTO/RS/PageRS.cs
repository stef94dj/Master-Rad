using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.DTO.RS
{
    public class PageRS<T>
    {
        public PageRS(IEnumerable<T> data, int pageCnt)
        {
            Data = data;
            PageCnt = pageCnt;
        }

        public IEnumerable<T> Data { get; set; }
        public int PageCnt { get; set; }
    }
}
