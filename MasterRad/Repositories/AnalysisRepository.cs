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
        AnalysisTestEntity Get(int testId);
        AnalysisTestStudentEntity GetAssignment(int studentId, int testId);
        AnalysisTestEntity GetWithTaskAndTemplate(int testId);
        IEnumerable<AnalysisTestStudentEntity> GetAssigned(int studentId);

        bool PaperExists(int studentId, int testId);
        bool CreatePaper(int studentId, int testId);

        bool Create(AnalysisCreateRQ request);
        bool UpdateName(UpdateNameRQ request);
        bool StatusNext(UpdateDTO request);
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

        public AnalysisTestEntity Get(int testId)
            => _context.AnalysisTests
                       .Where(t => t.Id == testId)
                       .AsNoTracking()
                       .SingleOrDefault();

        public AnalysisTestStudentEntity GetAssignment(int studentId, int testId)
            => _context.AnalysisTestStudents
                       .Where(ats => ats.StudentId == studentId && ats.AnalysisTestId == testId)
                       .Include(ats => ats.AnalysisTest)
                       .SingleOrDefault();

        public AnalysisTestEntity GetWithTaskAndTemplate(int testId)
            => _context.AnalysisTests
                       .Where(t => t.Id == testId)
                       .Include(a => a.SynthesisPaper)
                       .ThenInclude(sp => sp.SynthesisTestStudent)
                       .ThenInclude(sts => sts.SynthesisTest)
                       .ThenInclude(t => t.Task)
                       .ThenInclude(t => t.Template)
                       .AsNoTracking()
                       .SingleOrDefault();

        public IEnumerable<AnalysisTestStudentEntity> GetAssigned(int studentId)
            => _context.AnalysisTestStudents
                       .Include(ats => ats.AnalysisPaper)
                       .Include(ats => ats.AnalysisTest)
                       .Where(ats => ats.StudentId == studentId);

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

        public bool StatusNext(UpdateDTO request)
        {
            var currentStatus = Get(request.Id).Status;

            if (currentStatus == TestStatus.Completed)
                return false;

            var analysisTestEntity = new AnalysisTestEntity() //AutoMapper
            {
                Id = request.Id,
                TimeStamp = request.TimeStamp,
                Status = currentStatus + 1,
                DateModified = DateTime.UtcNow,
                ModifiedBy = "Current user - NOT IMPLEMENTED",
            };

            _context.AnalysisTests.Attach(analysisTestEntity);
            _context.Entry(analysisTestEntity).Property(x => x.Status).IsModified = true;
            _context.Entry(analysisTestEntity).Property(x => x.DateModified).IsModified = true;
            _context.Entry(analysisTestEntity).Property(x => x.ModifiedBy).IsModified = true;

            return _context.SaveChanges() == 1;
        }

        public bool PaperExists(int studentId, int testId)
        => _context.AnalysisPapers
                   .Include(ap => ap.AnalysisTestStudent)
                   .Where(ap => 
                            ap.AnalysisTestStudent != null
                            && ap.AnalysisTestStudent.StudentId == studentId
                            && ap.AnalysisTestStudent.AnalysisTestId == testId)
                   .Any();

        public bool CreatePaper(int studentId, int testId)
        {
            var analysisPaperEntity = new AnalysisPaperEntity() //AutoMapper
            {
                DateCreated = DateTime.UtcNow,
                CreatedBy = "Current user - NOT IMPLEMENTED",
                ATS_StudentId = studentId,
                ATS_AnalysisTestId = testId,
            };

            _context.AnalysisPapers.Add(analysisPaperEntity);
            return _context.SaveChanges() == 1;
        }
    }
}
