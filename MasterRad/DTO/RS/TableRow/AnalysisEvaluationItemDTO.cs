using MasterRad.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.DTO.RS.TableRow
{
    public class AnalysisEvaluationItemDTO
    {
        public AnalysisEvaluationItemDTO(AnalysisTestStudentEntity entity, AzureUserDTO studentDetail)
        {
            TakenTest = entity.TakenTest;
            foreach (var prog in entity.EvaluationProgress)
            {
                switch (prog.Type)
                {
                    case AnalysisEvaluationType.PrepareData:
                        PrepareDataProgress = prog.Progress;
                        break;
                    case AnalysisEvaluationType.FailingInput:
                        FailingInputProgress = prog.Progress;
                        break;
                    case AnalysisEvaluationType.QueryOutput:
                        QueryOutputProgress = prog.Progress;
                        break;
                    case AnalysisEvaluationType.CorrectOutput:
                        CorrectOutputProgress = prog.Progress;
                        break;
                }
            }

            StudentDetail = studentDetail;
        }
        public EvaluationProgress PrepareDataProgress { get; set; }
        public EvaluationProgress FailingInputProgress { get; set; }
        public EvaluationProgress QueryOutputProgress { get; set; }
        public EvaluationProgress CorrectOutputProgress { get; set; }

        public bool TakenTest { get; set; }
        public AzureUserDTO StudentDetail { get; set; }
    }
}
