using MasterRad.Entities;
using MasterRad.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.DTO.RS.Card
{
    public class InstanceDTO
    {
        public InstanceDTO(ExerciseInstanceEntity entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            TemplateName = entity.Template.Name;
            DateCreated = entity.DateCreated.ToJsonString();
        }

        public int Id { get; set; }

        public string Name { get; set; }
        public string TemplateName { get; set; }
        public string DateCreated { get; set; }
    }
}
