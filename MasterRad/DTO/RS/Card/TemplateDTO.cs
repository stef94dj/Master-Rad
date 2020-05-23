using MasterRad.Entities;
using MasterRad.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.DTO.RS.Card
{
    public class TemplateDTO
    {
        public TemplateDTO(TemplateEntity entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Description = entity.ModelDescription;
            DateCreated = entity.DateCreated.ToJsonString();
        }

        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string DateCreated { get; set; }
    }
}
