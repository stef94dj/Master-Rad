using MasterRad.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.DTO.RS.TableRow
{
    public class AssignedStudentDTO
    {
        public AssignedStudentDTO(BaseTestStudentEntity entity, AzureUserDTO userDetail)
        {
            StudentId = entity.StudentId;
            TimeStamp = entity.TimeStamp;

            FirstName = userDetail.FirstName;
            LastName = userDetail.LastName;
            Email = userDetail.Email;
        }

        public Guid StudentId { get; set; }
        public byte[] TimeStamp { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
