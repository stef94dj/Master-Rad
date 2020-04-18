using MasterRad.Entities;
using MasterRad.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.DTO.RS.TableRow
{
    public class AnalysisTestDTO
    {
        public AnalysisTestDTO(AnalysisTestEntity entity, AzureUserDTO createdByDetail, AzureUserDTO studentDetail)
        {
            Id = entity.Id;
            TimeStamp = entity.TimeStamp;

            Name = entity.Name;
            SynthesisTestName = entity.SynthesisTestStudent.SynthesisTest.Name;
            TaskName = entity.SynthesisTestStudent.SynthesisTest.Task.Name;
            TemplateName = entity.SynthesisTestStudent.SynthesisTest.Task.Template.Name;
            Status = (int)entity.Status;
            DateCreated = entity.DateCreated.ToJsonString();

            CreatedBy = createdByDetail;
            Student = studentDetail;
        }

        public int Id { get; set; }
        public byte[] TimeStamp { get; set; }

        public string Name { get; set; }
        public string SynthesisTestName { get; set; }
        public string TaskName { get; set; }
        public string TemplateName { get; set; }
        public int Status { get; set; }
        public string DateCreated { get; set; }
        public AzureUserDTO CreatedBy { get; set; }
        public AzureUserDTO Student { get; set; }
    }
}
