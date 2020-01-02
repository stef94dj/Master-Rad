using MasterRad.DTOs;
using MasterRad.Entities;
using MasterRad.Helpers;
using MasterRad.Models;
using MasterRad.Repositories;
using MasterRad.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : Controller
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ISynthesisRepository _synthesisRepository;
        private readonly IAnalysisRepository _analysisRepository;
        private readonly IMicrosoftSQL _microsoftSQLService;

        public StudentController
        (
            IStudentRepository studentRepository, 
            ISynthesisRepository synthesisRepository, 
            IAnalysisRepository analysisRepository,
            IMicrosoftSQL microsoftSQLService
        )
        {
            _studentRepository = studentRepository;
            _synthesisRepository = synthesisRepository;
            _analysisRepository = analysisRepository;
            _microsoftSQLService = microsoftSQLService;
        }

        [HttpPost, Route("search")]
        public ActionResult<IEnumerable<StudentEntity>> SearchStudents([FromBody] SearchStudentRQ body)
            => Ok(_studentRepository.SearchStudents(body));

        [HttpGet, Route("get/assigned/{testType}/{testId}")]
        public ActionResult<IEnumerable<BaseTestStudentEntity>> GetAssignedToTest([FromRoute]TestType testType, int testId)
        {
            switch (testType)
            {
                case TestType.Synthesis:
                    return Ok(_studentRepository.GetAssignedSynthesis(testId));
                case TestType.Analysis:
                    return Ok(_studentRepository.GetAssignedAnalysis(testId));
                default:
                    return StatusCode(500);
            }
        }

        [HttpPost, Route("assign")]
        public ActionResult<Result<bool>> AssignStudents([FromBody] AssignStudentsRQ body)
        {
            switch (body.TestType)
            {
                case TestType.Synthesis:
                    var synthesisEntity = _synthesisRepository.GetWithTaskAndTemplate(body.TestId);

                    if (synthesisEntity.Status >= TestStatus.Completed)
                        return StatusCode(500);

                    var synthesisExamDbNames = DatabaseNameHelper.SynthesisTestExam(body.StudentIds, synthesisEntity.Id);

                    var synthesisTemplateName = synthesisEntity.Task.Template.NameOnServer;
                    var synthesisCloneSuccess = _microsoftSQLService.CloneDatabases(synthesisTemplateName, synthesisExamDbNames.Select(snp => snp.Value), false);

                    synthesisExamDbNames = synthesisExamDbNames.Where(x => synthesisCloneSuccess.Contains(x.Value));

                    var synthesisAssigned = _studentRepository.AssignSynthesisTest(synthesisExamDbNames, body.TestId);
                    if (synthesisAssigned != body.StudentIds.Count())
                        return Result<bool>.Fail("One or more students have not been assigned");
                    else
                        return Result<bool>.Success(true);
                case TestType.Analysis:
                    if (_analysisRepository.Get(body.TestId).Status >= TestStatus.Completed)
                        return StatusCode(500);

                    var analysisEntity = _analysisRepository.GetWithTaskAndTemplate(body.TestId);

                    var analysisExamDbNames = DatabaseNameHelper.AnalysisTestExam(body.StudentIds, analysisEntity.Id);

                    var analysisTemplateName = analysisEntity.SynthesisPaper.SynthesisTestStudent.SynthesisTest.Task.Template.NameOnServer;
                    var analysisCloneSuccess = _microsoftSQLService.CloneDatabases(analysisTemplateName, analysisExamDbNames.Select(snp => snp.Value), false);

                    analysisExamDbNames = analysisExamDbNames.Where(x => analysisCloneSuccess.Contains(x.Value));

                    var analysisAssigned = _studentRepository.AssignAnalysisTest(analysisExamDbNames, body.TestId);
                    if (analysisAssigned != body.StudentIds.Count())
                        return Result<bool>.Fail("One or more students have not been assigned");
                    else
                        return Result<bool>.Success(true);
                default:
                    return StatusCode(500);
            }
        }

        [HttpPost, Route("remove/assigned")]
        public ActionResult RemoveFromTest([FromBody] RemoveAssignedRQ body)
        {
            switch (body.TestType)
            {
                case TestType.Synthesis:
                    if (_synthesisRepository.Get(body.TestId).Status >= TestStatus.InProgress)
                        return StatusCode(500);

                    var assignment = _synthesisRepository.GetAssignment(body.StudentId, body.TestId);

                    var deleteSuccess = _microsoftSQLService.DeleteDatabaseIfExists(assignment.NameOnServer);
                    if (!deleteSuccess)
                        return StatusCode(500);

                    _studentRepository.RemoveSynthesis(body.StudentId, body.TimeStamp, body.TestId);
                    break;
                case TestType.Analysis:
                    throw new NotImplementedException();
                //if (_analysisRepository.Get(body.TestId).Status >= TestStatus.InProgress)
                //    return StatusCode(500);

                //_studentRepository.AssignAnalysisTest(body.StudentId, body.TimeStamp, body.TestId);
                //break;
                default:
                    return StatusCode(500);
            }

            return Ok(true);
        }
    }
}
