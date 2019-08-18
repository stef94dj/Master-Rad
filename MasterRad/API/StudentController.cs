using MasterRad.DTOs;
using MasterRad.Entities;
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

        public StudentController(IStudentRepository studentRepository, ISynthesisRepository synthesisRepository)
        {
            _studentRepository = studentRepository;
            _synthesisRepository = synthesisRepository;
        }

        [HttpPost, Route("search")]
        public ActionResult<IEnumerable<StudentEntity>> SearchStudents([FromBody] SearchStudentRQ body)
        {

            var res = _studentRepository.SearchStudents(body);
            return Ok(res);
        }

        [HttpGet, Route("get/assigned/{testType}/{testId}")]
        public ActionResult<IEnumerable<StudentEntity>> GetAssignedToTest([FromRoute]TestType testType, int testId)
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
        public ActionResult<IEnumerable<BaseTestStudentEntity>> AssignStudents([FromBody] AssignStudentsRQ body)
        {
            switch (body.TestType)
            {
                case TestType.Synthesis:
                    if (_synthesisRepository.Get(body.TestId).Status >= TestStatus.Completed)
                        return StatusCode(500);

                    return Ok(_studentRepository.AssignSynthesisTest(body.StudentIds, body.TestId));
                case TestType.Analysis:
                    throw new NotImplementedException();
                    //if (_analysisRepository.Get(body.TestId).Status >= TestStatus.Completed)
                    //    return StatusCode(500);

                    return Ok(_studentRepository.AssignAnalysisTest(body.StudentIds, body.TestId));
                default:
                    return StatusCode(500);
            }
        }
    }
}
