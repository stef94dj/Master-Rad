using MasterRad.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.DTO.RS.TableRow
{
    public class SynthesisEvaluationItemDTO
    {
        public SynthesisEvaluationItemDTO(SynthesisTestStudentEntity entity, AzureUserDTO studentDetail)
        {

            TakenTest = entity.TakenTest;
            PublicDataProgress = entity.EvaluationProgress.Single(ep => !ep.IsSecretDataUsed).Progress;
            SecretDataProgress = entity.EvaluationProgress.Single(ep => ep.IsSecretDataUsed).Progress;

            StudentDetail = studentDetail;
        }

        public bool TakenTest { get; set; }
        public EvaluationProgress PublicDataProgress { get; set; }
        public EvaluationProgress SecretDataProgress { get; set; }

        public AzureUserDTO StudentDetail { get; set; }
    }
}
