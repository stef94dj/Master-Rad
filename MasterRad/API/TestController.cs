using MasterRad.DTOs;
using MasterRad.Entities;
using MasterRad.Models.DTOs;
using MasterRad.Repositories;
using MasterRad.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ISynthesisRepository _synthesisRepository;
        private readonly IAnalysisRepository _analysisRepository;

        public TestController
        (
            ISynthesisRepository synthesisRepository,
            IAnalysisRepository analysisRepository
        )
        {
            _synthesisRepository = synthesisRepository;
            _analysisRepository = analysisRepository;
        }

        [HttpGet, Route("get/student/assigned")]
        public ActionResult<IEnumerable<StudentTestDto>> GetTestsAssignedToStudent()
        {
            var userId = 1;

            var res = new List<StudentTestDto>();

            //AutoMapper with LINQ ProjectTo:
            var sytnhesisTests = _synthesisRepository.GetAssigned(userId);
            if (sytnhesisTests != null && sytnhesisTests.Any())
                res.AddRange(sytnhesisTests.Select(sts => new StudentTestDto(sts)));

            var analysisTests = _analysisRepository.GetAssigned(userId);
            if (analysisTests != null && analysisTests.Any())
                res.AddRange(analysisTests.Select(ats => new StudentTestDto(ats)));

            return res;
        }
    }
}
