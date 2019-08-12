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

        public TestController(ISynthesisRepository synthesisRepository)
        {
            _synthesisRepository = synthesisRepository;
        }

        [HttpGet, Route("get/assigned")]
        public ActionResult<IEnumerable<SynthesisTestEntity>> GetTests()
        {
            var res = _synthesisRepository.GetAssigned(1);
            return Ok(res);
        }

    }
}
