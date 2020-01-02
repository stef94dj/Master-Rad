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
    public class AnalysisController : ControllerBase
    {
        private readonly IUser _userService;
        private readonly IAnalysisRepository _analysisRepository;

        public AnalysisController(IUser userService, IAnalysisRepository analysisRepository)
        {
            _userService = userService;
            _analysisRepository = analysisRepository;
        }

        [HttpGet, Route("get")]
        public ActionResult<IEnumerable<AnalysisTestEntity>> GetTests()
            => Ok(_analysisRepository.Get());

        [HttpPost, Route("create/test")]
        public ActionResult<bool> CreateTest([FromBody] AnalysisCreateRQ body)
           => Ok(_analysisRepository.Create(body));

        [HttpPost, Route("update/name")]
        public ActionResult<bool> UpdateTestName([FromBody] UpdateNameRQ body)
            => Ok(_analysisRepository.UpdateName(body));
    }
}
