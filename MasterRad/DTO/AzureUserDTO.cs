using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.DTO
{
    public class AzureUserDTO
    {

        public AzureUserDTO(User graphUser)
        {
            MicrosoftId = Guid.Parse(graphUser.Id);
            MicrososftUserName = graphUser.UserPrincipalName;

            FirstName = graphUser.GivenName;
            LastName = graphUser.Surname;
            Email = graphUser.Mail;
        }

        public Guid MicrosoftId { get; set; }
        public string MicrososftUserName { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
