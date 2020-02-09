﻿using MasterRad.Models.ViewModels;
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
        {
            var testStudentEntity = _analysisRepository.GetAssignment(_userService.UserId, testId);

            var outputTablesDb = _config.GetValue<string>("DbAdminConnection:DbName");

            var vm = new AnalysisExamVM()
            {
                Title = $"Task '{testStudentEntity.AnalysisTest.Name}'",
                FailingInputVM = new ModifyDatabasePartialVM()
                {
                    NameOnServer = testStudentEntity.InputNameOnServer
                },
                StudentOutputVM = new ModifyTablePartialVM()
                {
                    NameOnServer = outputTablesDb,
                    TableName = testStudentEntity.StudentOutputNameOnServer
                },
                CorrectOutputVM = new ModifyTablePartialVM()
                {
                    NameOnServer = outputTablesDb,
                    TableName = testStudentEntity.TeacherOutputNameOnServer
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
                TestType = testType,
                JobId = 123
            };
            return View(vm);
        }
    }
}
