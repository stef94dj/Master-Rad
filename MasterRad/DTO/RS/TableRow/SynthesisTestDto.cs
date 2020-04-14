using MasterRad.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.DTO.RS.TableRow
{
    public class SynthesisTestDTO
    {
        public SynthesisTestDTO(SynthesisTestEntity entity, AzureUserDTO userDetail)
        {
            Id = entity.Id;
            TimeStamp = entity.TimeStamp;

            Name = entity.Name;
            TaskName = entity.Task.Name;
            TemplateName = entity.Task.Template.Name;
            Status = (int)entity.Status;
            DateCreated = entity.DateCreated.HasValue ? entity.DateCreated.Value.ToString(Constants.JSDateFormat) : "N/A";
            CreatedBy = userDetail;
        }

        public int Id { get; set; }
        public byte[] TimeStamp { get; set; }

        public string Name { get; set; }
        public string TaskName { get; set; }
        public string TemplateName { get; set; }
        public int Status { get; set; }
        public string DateCreated { get; set; }
        public AzureUserDTO CreatedBy { get; set; }
    }
}
