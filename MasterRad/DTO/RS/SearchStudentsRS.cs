using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.DTO.RS
{
    public class SearchStudentsRS
    {
        public SearchStudentsRS(IEnumerable<StudentDTO> students, string nextPageUrl)
        {
            Students = students;
            NextPageUrl = nextPageUrl;
        }

        public IEnumerable<StudentDTO> Students { get; set; }
        public string NextPageUrl { get; set; }
    }
}
