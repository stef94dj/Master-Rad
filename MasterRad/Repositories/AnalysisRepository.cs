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
        AnalysisTestEntity GetWithTaskTemplateAndSolutionFormat(int testId);
        IEnumerable<AnalysisTestStudentEntity> GetAssigned(int studentId);

        bool Create(AnalysisCreateRQ request);
        bool UpdateName(UpdateNameRQ request);
        bool StatusNext(UpdateDTO request);

        IEnumerable<AnalysisTestStudentEntity> GetPapers(int testId);
        bool MarkExamAsTaken(int testId, int studentId, byte[] timeStamp);
        AnalysisTestStudentEntity GetEvaluationData(int testId, int studentId);
        bool SaveProgress(AnalysisTestStudentEntity entity, AnalysisEvaluationType type, EvaluationProgress status, string message = null);
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
                   .Include(at => at.SynthesisTestStudent)
                   .ThenInclude(sts => sts.SynthesisTest)
                   .ThenInclude(st => st.Task)
                   .ThenInclude(t => t.Template)
                   .Include(at => at.SynthesisTestStudent)
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
                       .AsNoTracking()
                       .SingleOrDefault();

        public AnalysisTestEntity GetWithTaskTemplateAndSolutionFormat(int testId)
            => _context.AnalysisTests
                       .Where(t => t.Id == testId)
                       .Include(a => a.SynthesisTestStudent)
                       .ThenInclude(sts => sts.SynthesisTest)
                       .ThenInclude(t => t.Task)
                       .ThenInclude(t => t.Template)
                       .Include(a => a.SynthesisTestStudent)
                       .ThenInclude(sts => sts.SynthesisTest)
                       .ThenInclude(t => t.Task)
                       .ThenInclude(t => t.SolutionColumns)
                       .AsNoTracking()
                       .SingleOrDefault();

        public IEnumerable<AnalysisTestStudentEntity> GetAssigned(int studentId)
            => _context.AnalysisTestStudents
                       .Include(sts => sts.EvaluationProgress)
                       .Include(ats => ats.AnalysisTest)
                       .Where(ats => ats.StudentId == studentId);

        public bool Create(AnalysisCreateRQ request)
        {
            var analysisTestEntity = new AnalysisTestEntity() //AutoMapper
            {
                Name = request.Name,
                STS_StudentId = request.StudentId,
                STS_SynthesisTestId = request.SynthesisTestId,
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

        public IEnumerable<AnalysisTestStudentEntity> GetPapers(int testId)
            => _context.AnalysisTestStudents
                       .Include(sts => sts.EvaluationProgress)
                       .Include(sts => sts.Student)
                       .Where(sts => sts.AnalysisTestId == testId);

        public bool MarkExamAsTaken(int testId, int studentId, byte[] timeStamp)
        {
            var analysisTestEntity = new AnalysisTestStudentEntity() //AutoMapper
            {
                StudentId = studentId,
                AnalysisTestId = testId,
                TimeStamp = timeStamp,
                TakenTest = true,
                DateModified = DateTime.UtcNow,
                ModifiedBy = "Current user - NOT IMPLEMENTED",
            };

            _context.AnalysisTestStudents.Attach(analysisTestEntity);
            _context.Entry(analysisTestEntity).Property(x => x.TakenTest).IsModified = true;
            _context.Entry(analysisTestEntity).Property(x => x.DateModified).IsModified = true;
            _context.Entry(analysisTestEntity).Property(x => x.ModifiedBy).IsModified = true;

            return _context.SaveChanges() == 1;
        }

        public AnalysisTestStudentEntity GetEvaluationData(int testId, int studentId)
        => _context.AnalysisTestStudents.Where(ats => ats.AnalysisTestId == testId && ats.StudentId == studentId)
                                        .Include(ats => ats.EvaluationProgress)
                                        .Include(ats => ats.AnalysisTest)
                                        .ThenInclude(sp => sp.SynthesisTestStudent)
                                        .ThenInclude(sts => sts.SynthesisTest)
                                        .ThenInclude(st => st.Task)
                                        .ThenInclude(t => t.SolutionColumns)
                                        .Single();

        public bool SaveProgress(AnalysisTestStudentEntity entity, AnalysisEvaluationType type, EvaluationProgress status, string message = null)
        {
            var progressEntity = entity.EvaluationProgress.Single(x => x.Type == type);

            progressEntity.DateModified = DateTime.UtcNow;
            progressEntity.ModifiedBy = "Current user - NOT IMPLEMENTED";
            progressEntity.Progress = status;
            progressEntity.Message = message;

            var affectedRecords = _context.SaveChanges();
            return affectedRecords == 1;
        }
    }
}
