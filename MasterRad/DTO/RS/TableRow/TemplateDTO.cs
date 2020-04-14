using MasterRad.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.DTO.RS.TableRow
{
    public class TemplateDTO
    {
        public TemplateDTO(TemplateEntity te, AzureUserDTO userDetail)
        {
            Id = te.Id;
            TimeStamp = te.TimeStamp;

            Name = te.Name;
            Description = te.ModelDescription;
            NameOnServer = te.NameOnServer;
            DateCreated = te.DateCreated.HasValue ? te.DateCreated.Value.ToString(Constants.JSDateFormat) : "N/A"; 
            CreatedBy = userDetail;
        }

        public int Id { get; set; }
        public byte[] TimeStamp { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string NameOnServer { get; set; }
        public string DateCreated { get; set; }
        public AzureUserDTO CreatedBy { get; set; }
    }
}
