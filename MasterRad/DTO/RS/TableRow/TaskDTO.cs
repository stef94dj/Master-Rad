using MasterRad.Entities;
using MasterRad.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.DTO.RS.TableRow
{
    public class TaskDTO
    {
        public TaskDTO(TaskEntity taskEntity, AzureUserDTO userDetail)
        {
            Id = taskEntity.Id;
            TimeStamp = taskEntity.TimeStamp;

            Name = taskEntity.Name;
            TemplateName = taskEntity.Template.Name;
            Description = taskEntity.Description;
            DateCreated = taskEntity.DateCreated.ToJsonString();
            CreatedBy = userDetail;
        }

        public int Id { get; set; }
        public byte[] TimeStamp { get; set; }

        public string Name { get; set; }
        public string TemplateName { get; set; }
        public string Description { get; set; }
        public string DateCreated { get; set; }
        public AzureUserDTO CreatedBy { get; set; }
    }
}
