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
            var testAssignment = _synthesisRepository.GetAssignment(_userService.UserId, testId);

            if (testAssignment == null)
                return Unauthorized();

            var vm = new SynthesisExamVM()
            {
                TestId = testId,
                NameOnServer = testAssignment.NameOnServer
            };

            return View(vm);
        }
        public IActionResult AnalysisExam(int testId)
        {
            return View();
        }

    }
}
