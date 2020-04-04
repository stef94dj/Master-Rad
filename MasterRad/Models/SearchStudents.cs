using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Models
{
    public class SearchStudentsRQ
    {
        public SearchStudentsRQ(int pageSize)
        {
            PageSize = pageSize;
        }

        public int PageSize { get; set; }
        public string FirstNameFilter { get; set; }
        public string LastNameFilter { get; set; }
        public string EmailFilter { get; set; }
    }

    public class SearchStudentsRS
    {
        public SearchStudentsRS(IEnumerable<Student> students, string nextPageUrl)
        {
            Students = students;
            NextPageUrl = nextPageUrl;
        }

        public IEnumerable<Student> Students { get; set; }
        public string NextPageUrl { get; set; }
    }

    public class Student
    {
        public Student(string microsoftId, string firstName, string lastName, string email, string microsoftUserName)
        {
            MicrosoftId = Guid.Parse(microsoftId);
            MicrososftUserName = microsoftUserName;

            FirstName = firstName;
            LastName = lastName;
            Email = email;
        }

        public Guid MicrosoftId { get; set; }
        public string MicrososftUserName { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
