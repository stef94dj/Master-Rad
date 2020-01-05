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

        [HttpGet, Route("get/student/assigned")]
        public ActionResult<IEnumerable<SynthesisTestEntity>> GetTestsAssignedToStudent()
        {
            var res = _synthesisRepository.GetAssigned(1); //ukljuci i analysis tests (LINQ project to)
            return Ok(res);
        }

        //[HttpGet, Route("get/synthesis")]
        //public ActionResult<IEnumerable<SynthesisTestEntity>> GetSynthesis()
        //    => Ok(_synthesisRepository.Get());

        //[HttpGet, Route("get/analysis")]
        //public ActionResult<IEnumerable<SynthesisTestEntity>> GetAnalysis()
        //{
        //    throw new NotImplementedException();
        //    //var res = _synthesisRepository.Get(); //ukljuci i analysis tests (LINQ project to)
        //    //return Ok(res);
        //}
    }
}
