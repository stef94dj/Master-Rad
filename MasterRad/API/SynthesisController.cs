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
    public class SynthesisController : ControllerBase
    {
        private readonly ISynthesisRepository _synthesisRepository;

        public SynthesisController(ISynthesisRepository synthesisRepository)
        {
            _synthesisRepository = synthesisRepository;
        }

        [HttpGet, Route("get")]
        public ActionResult<IEnumerable<SynthesisTestEntity>> GetTests()
        {
            var res = _synthesisRepository.Get();
            return Ok(res);
        }

        [HttpPost, Route("create/test")]
        public ActionResult<SynthesisTestEntity> CreateTest([FromBody] SynthesisCreateRQ body)
        {
            var res = _synthesisRepository.Create(body);
            return Ok(res);
        }

        [HttpPost, Route("delete/test")]
        public ActionResult<SynthesisTestEntity> DeleteTest([FromBody] DeleteDTO body)
        {
             _synthesisRepository.Delete(body);
            return Ok();
        }

        [HttpPost, Route("update/name")]
        public ActionResult<SynthesisTestEntity> DeleteTest([FromBody] UpdateNameRQ body)
        {
            var res = _synthesisRepository.UpdateName(body);
            return Ok(res);
        }
    }
}
