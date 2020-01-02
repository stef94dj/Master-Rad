using MasterRad.DTOs;
using MasterRad.Entities;
using MasterRad.Models;
using MasterRad.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MasterRad.Repositories
{
    public interface IAnalysisRepository
    {
        IEnumerable<AnalysisTestEntity> Get();
        bool Create(AnalysisCreateRQ request);
        bool UpdateName(UpdateNameRQ request);
    }

    public class AnalysisRepository : IAnalysisRepository
    {
        private readonly Context _context;

        public AnalysisRepository(Context context)
        {
            _context = context;
        }

        public IEnumerable<AnalysisTestEntity> Get()
        => _context.AnalysisTests
                   .Include(a => a.SynthesisPaper)
                   .ThenInclude(sp => sp.SynthesisTestStudent)
                   .ThenInclude(sts => sts.SynthesisTest)
                   .ThenInclude(st => st.Task)
                   .ThenInclude(t => t.Template)
                   .Include(a => a.SynthesisPaper)
                   .ThenInclude(sp => sp.SynthesisTestStudent)
                   .ThenInclude(sts => sts.Student);

        public bool Create(AnalysisCreateRQ request)
        {
            var analysisTestEntity = new AnalysisTestEntity() //AutoMapper
            {
                Name = request.Name,
                SynthesisPaperId = request.SynthesisPaperId,
                Status = TestStatus.Scheduled,
                DateCreated = DateTime.UtcNow,
                CreatedBy = "Current user - NOT IMPLEMENTED",
            };

            _context.AnalysisTests.Add(analysisTestEntity);
            return _context.SaveChanges() == 1;
        }

        public bool UpdateName(UpdateNameRQ request)
        {
            var analysisTestEntity = new AnalysisTestEntity() //AutoMapper
            {
                Id = request.Id,
                TimeStamp = request.TimeStamp,
                Name = request.Name,
                DateModified = DateTime.UtcNow,
                ModifiedBy = "Current user - NOT IMPLEMENTED",
            };

            _context.AnalysisTests.Attach(analysisTestEntity);
            _context.Entry(analysisTestEntity).Property(x => x.Name).IsModified = true;
            _context.Entry(analysisTestEntity).Property(x => x.DateModified).IsModified = true;
            _context.Entry(analysisTestEntity).Property(x => x.ModifiedBy).IsModified = true;

            return _context.SaveChanges() == 1;
        }
    }
}
