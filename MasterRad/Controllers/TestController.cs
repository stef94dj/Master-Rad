using MasterRad.Models.ViewModels;
using MasterRad.Repositories;
using MasterRad.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Controllers
{
    public class TestController : Controller
    {
        private readonly IUser _userService;
        private readonly ISynthesisRepository _synthesisRepository;

        public TestController(IUser userService, ISynthesisRepository synthesisRepository)
        {
            _userService = userService;
            _synthesisRepository = synthesisRepository;
        }

        public IActionResult SynthesisExam(int testId)
        {
            var testAssignment = _synthesisRepository.GetAssignmentWithTaskAndTemplate(_userService.UserId, testId);

            if (testAssignment == null)
                return Unauthorized();

            var stsTimeStamp = testAssignment.SynthesisPaper?.TimeStamp;
            var vm = new SynthesisExamVM()
            {
                TestId = testId,
                SynthesisPaperId = testAssignment.SynthesisPaper?.Id ?? 0,
                SynthesisPaperTimeStamp = stsTimeStamp == null ? string.Empty : Convert.ToBase64String(stsTimeStamp),
                NameOnServer = testAssignment.NameOnServer,
                SqlScript = testAssignment.SynthesisPaper?.SqlScript ?? string.Empty,
                TaskDescription = testAssignment.SynthesisTest.Task.Description,
                ModelDescription = testAssignment.SynthesisTest.Task.Template.ModelDescription
            };

            return View(vm);
        }
        public IActionResult AnalysisExam(int testId)
            => View();

        public IActionResult AssignStudents(int testId, TestType testType)
        {
            switch (testType)
            {
                case TestType.Synthesis:
                    if (_synthesisRepository.Get(testId).Status >= TestStatus.Completed)
                        return StatusCode(500);
                    break;
                case TestType.Analysis:
                    throw new NotImplementedException();
                    //if (_analysisRepository.Get(testId).Status >= TestStatus.Completed)
                    //    return StatusCode(500);
                    //break;
                default:
                    return StatusCode(500);
            }

            var vm = new AssignStudentsVM
            {
                TestId = testId,
                TestType = testType
            };
            return View(vm);
        }

        public IActionResult Results(int testId, TestType testType)
        {
            switch (testType)
            {
                case TestType.Synthesis:
                    
                    break;
                case TestType.Analysis:
                    throw new NotImplementedException();
                default:
                    return StatusCode(500);
            }

            var vm = new TestResultsVM
            {
                TestId = testId,
                TestType = testType
            };
            return View(vm);
        }
    }
}
