using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.DTO.RS
{
    public class PageRS<T>
    {
        public PageRS(IEnumerable<T> data, int pageCnt, int pageNo)
        {
            Data = data;
            PageCnt = pageCnt;
            PageNo = pageNo;
        }

        public IEnumerable<T> Data { get; set; }
        public int PageCnt { get; set; }
        public int PageNo { get; set; }
    }
}
