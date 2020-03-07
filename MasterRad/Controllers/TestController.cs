using MasterRad.Models.ViewModels;
using MasterRad.Repositories;
using MasterRad.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        private readonly IAnalysisRepository _analysisRepository;
        private readonly IConfiguration _config;

        public TestController
        (
            IUser userService,
            ISynthesisRepository synthesisRepository,
            IAnalysisRepository analysisRepository,
            IConfiguration config
        )
        {
            _userService = userService;
            _synthesisRepository = synthesisRepository;
            _analysisRepository = analysisRepository;
            _config = config;
        }

        public IActionResult SynthesisExam(int testId, byte[] timeStamp)
        {
            var stsEntity = _synthesisRepository.GetAssignmentWithTaskAndTemplate(_userService.UserId, testId);

            if (stsEntity == null)
                return Unauthorized();

            if (!stsEntity.TakenTest)
                _synthesisRepository.MarkExamAsTaken(testId, _userService.UserId, timeStamp);

            var vm = new SynthesisExamVM()
            {
                TestId = stsEntity.SynthesisTestId,
                StudentId = stsEntity.StudentId,
                TimeStamp = Convert.ToBase64String(stsEntity.TimeStamp),
                NameOnServer = stsEntity.NameOnServer,
                SqlScript = stsEntity.SqlScript ?? string.Empty,
                TaskDescription = stsEntity.SynthesisTest.Task.Description,
                ModelDescription = stsEntity.SynthesisTest.Task.Template.ModelDescription
            };

            return View(vm);
        }
        public IActionResult AnalysisExam(int testId, byte[] timeStamp)
        {
            var atsEntity = _analysisRepository.GetAssignment(_userService.UserId, testId);

            if (atsEntity == null)
                return Unauthorized();

            if (!atsEntity.TakenTest)
                _analysisRepository.MarkExamAsTaken(testId, _userService.UserId, timeStamp);

            var outputTablesDb = _config.GetValue<string>("DbAdminConnection:DbName");

            var vm = new AnalysisExamVM()
            {
                Title = $"Task '{atsEntity.AnalysisTest.Name}'",
                FailingInputVM = new ModifyDatabasePartialVM()
                {
                    NameOnServer = atsEntity.InputNameOnServer
                },
                StudentOutputVM = new ModifyTablePartialVM()
                {
                    NameOnServer = outputTablesDb,
                    TableName = atsEntity.StudentOutputNameOnServer
                },
                CorrectOutputVM = new ModifyTablePartialVM()
                {
                    NameOnServer = outputTablesDb,
                    TableName = atsEntity.TeacherOutputNameOnServer
                }
            };

            return View("~/Views/Test/AnalysisExam.cshtml", vm);
        }

        public IActionResult AssignStudents(int testId, TestType testType)
        {
            switch (testType)
            {
                case TestType.Synthesis:
                    if (_synthesisRepository.Get(testId).Status >= TestStatus.Completed)
                        return StatusCode(500);
                    break;
                case TestType.Analysis:
                    if (_analysisRepository.Get(testId).Status >= TestStatus.Completed)
                        return StatusCode(500);
                    break;
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
            if (testType != TestType.Synthesis && testType != TestType.Analysis)
                throw new Exception($"Invalid test type '{testType}'");

            var vm = new TestResultsVM
            {
                TestId = testId,
                TestType = testType
            };
            return View(vm);
        }
    }
}
