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

            foreach (var prog in entity.EvaluationProgress)
            {
                if (prog.IsSecretDataUsed)
                    SecretDataProgress = prog.Progress;
                else
                    PublicDataProgress = prog.Progress;
            }

            StudentDetail = studentDetail;
        }

        public bool TakenTest { get; set; }
        public EvaluationProgress PublicDataProgress { get; set; }
        public EvaluationProgress SecretDataProgress { get; set; }

        public AzureUserDTO StudentDetail { get; set; }
    }
}
