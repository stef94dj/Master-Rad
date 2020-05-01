using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.DTO.RQ
{
    public class SearchPaginatedRQ
    {
        public string SortBy { get; set; }
        public bool SortDescending { get; set; }
        public IEnumerable<KeyValuePair<string,string>> Filters { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
