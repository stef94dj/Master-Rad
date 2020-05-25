using MasterRad.Entities;
using MasterRad.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.DTO.RS.TableRow
{
    public class TemplateDTO
    {
        public TemplateDTO(TemplateEntity entity, AzureUserDTO userDetail)
        {
            Id = entity.Id;
            TimeStamp = entity.TimeStamp;

            Name = entity.Name;
            Description = entity.ModelDescription;
            NameOnServer = entity.NameOnServer;
            IsPublic = entity.IsPublic;
            DateCreated = entity.DateCreated.ToJsonString(); 
            CreatedBy = userDetail;
        }

        public int Id { get; set; }
        public byte[] TimeStamp { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string NameOnServer { get; set; }
        public string DateCreated { get; set; }
        public AzureUserDTO CreatedBy { get; set; }
        public bool IsPublic { get; set; }
    }
}
