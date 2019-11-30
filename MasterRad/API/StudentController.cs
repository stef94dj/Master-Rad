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
        private readonly IMicrosoftSQL _microsoftSQLService;

        public StudentController(IStudentRepository studentRepository, ISynthesisRepository synthesisRepository, IMicrosoftSQL microsoftSQLService)
        {
            _studentRepository = studentRepository;
            _synthesisRepository = synthesisRepository;
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
                    var test = _synthesisRepository.GetWithTaskAndTemplate(body.TestId);

                    if (test.Status >= TestStatus.Completed)
                        return StatusCode(500);

                    var studenDbNamePairs = DatabaseNameHelper.SynthesisTestExam(body.StudentIds, test.Id);

                    var succesfullyCloned = _microsoftSQLService.CloneDatabases(test.Task.Template.NameOnServer, studenDbNamePairs.Select(snp => snp.Value), false);

                    studenDbNamePairs = studenDbNamePairs.Where(x => succesfullyCloned.Contains(x.Value));

                    var assigned = _studentRepository.AssignSynthesisTest(studenDbNamePairs, body.TestId);

                    if (assigned != body.StudentIds.Count())
                        return Result<bool>.Fail("One or more students have not been assigned");
                    else
                        return Result<bool>.Success(true);
                case TestType.Analysis:
                    throw new NotImplementedException();
                    //if (_analysisRepository.Get(body.TestId).Status >= TestStatus.Completed)
                    //    return StatusCode(500);

                    return Ok(_studentRepository.AssignAnalysisTest(body.StudentIds, body.TestId));
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
