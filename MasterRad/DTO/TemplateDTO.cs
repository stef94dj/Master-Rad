using MasterRad.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.DTO
{
    public class TemplateDTO
    {
        public TemplateDTO(TemplateEntity te, AzureUserDTO userDetail)
        {
            Id = te.Id;
            Name = te.Name;
            ModelDescription = te.ModelDescription;
            NameOnServer = te.NameOnServer;
            TimeStamp = te.TimeStamp;
            DateCreated = te.DateCreated.HasValue ? te.DateCreated.Value.ToString(Constants.JSDateFormat) : "N/A"; 

            CreatedBy = userDetail;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string ModelDescription { get; set; }
        public string NameOnServer { get; set; }
        public byte[] TimeStamp { get; set; }
        public string DateCreated { get; set; }
        public AzureUserDTO CreatedBy { get; set; }
    }
}
