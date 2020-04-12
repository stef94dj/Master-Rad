﻿using MasterRad.DTO;
using MasterRad.Entities;
using MasterRad.DTO.RQ;
using MasterRad.DTO.RS;
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
        AnalysisTestStudentEntity GetAssignment(Guid studentId, int testId);
        AnalysisTestEntity GetWithTaskTemplateAndSolutionFormat(int testId);
        IEnumerable<AnalysisTestStudentEntity> GetAssigned(Guid studentId);

        bool Create(AnalysisCreateRQ request, Guid userId);
        bool UpdateName(UpdateNameRQ request, Guid userId);
        bool StatusNext(UpdateDTO request, Guid userId);

        IEnumerable<AnalysisTestStudentEntity> GetPapers(int testId);
        bool MarkExamAsTaken(int testId, Guid studentId, byte[] timeStamp);
        AnalysisTestStudentEntity GetEvaluationData(int testId, Guid studentId);
        bool SaveProgress(AnalysisTestStudentEntity entity, AnalysisEvaluationType type, EvaluationProgress status, Guid userId, string message = null);
        bool TestExists(string name);
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

        public AnalysisTestStudentEntity GetAssignment(Guid studentId, int testId)
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

        public IEnumerable<AnalysisTestStudentEntity> GetAssigned(Guid studentId)
            => _context.AnalysisTestStudents
                       .Include(sts => sts.EvaluationProgress)
                       .Include(ats => ats.AnalysisTest)
                       .Where(ats => ats.StudentId == studentId);

        public bool Create(AnalysisCreateRQ request, Guid userId)
        {
            var analysisTestEntity = new AnalysisTestEntity() //AutoMapper
            {
                Name = request.Name,
                STS_StudentId = request.StudentId,
                STS_SynthesisTestId = request.SynthesisTestId,
                Status = TestStatus.Scheduled,
                DateCreated = DateTime.UtcNow,
                CreatedBy = userId,
            };

            _context.AnalysisTests.Add(analysisTestEntity);
            return _context.SaveChanges() == 1;
        }

        public bool UpdateName(UpdateNameRQ request, Guid userId)
        {
            var analysisTestEntity = new AnalysisTestEntity() //AutoMapper
            {
                Id = request.Id,
                TimeStamp = request.TimeStamp,
                Name = request.Name,
                DateModified = DateTime.UtcNow,
                ModifiedBy = userId,
            };

            _context.AnalysisTests.Attach(analysisTestEntity);
            _context.Entry(analysisTestEntity).Property(x => x.Name).IsModified = true;
            _context.Entry(analysisTestEntity).Property(x => x.DateModified).IsModified = true;
            _context.Entry(analysisTestEntity).Property(x => x.ModifiedBy).IsModified = true;

            return _context.SaveChanges() == 1;
        }

        public bool StatusNext(UpdateDTO request, Guid userId)
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
                ModifiedBy = userId,
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

        public bool MarkExamAsTaken(int testId, Guid studentId, byte[] timeStamp)
        {
            var analysisTestEntity = new AnalysisTestStudentEntity() //AutoMapper
            {
                StudentId = studentId,
                AnalysisTestId = testId,
                TimeStamp = timeStamp,
                TakenTest = true,
                DateModified = DateTime.UtcNow,
                ModifiedBy = studentId,
            };

            _context.AnalysisTestStudents.Attach(analysisTestEntity);
            _context.Entry(analysisTestEntity).Property(x => x.TakenTest).IsModified = true;
            _context.Entry(analysisTestEntity).Property(x => x.DateModified).IsModified = true;
            _context.Entry(analysisTestEntity).Property(x => x.ModifiedBy).IsModified = true;

            return _context.SaveChanges() == 1;
        }

        public AnalysisTestStudentEntity GetEvaluationData(int testId, Guid studentId)
        => _context.AnalysisTestStudents.Where(ats => ats.AnalysisTestId == testId && ats.StudentId == studentId)
                                        .Include(ats => ats.EvaluationProgress)
                                        .Include(ats => ats.AnalysisTest)
                                        .ThenInclude(sp => sp.SynthesisTestStudent)
                                        .ThenInclude(sts => sts.SynthesisTest)
                                        .ThenInclude(st => st.Task)
                                        .ThenInclude(t => t.SolutionColumns)
                                        .Single();

        public bool SaveProgress(AnalysisTestStudentEntity entity, AnalysisEvaluationType type, EvaluationProgress status, Guid userId, string message = null)
        {
            var progressEntity = entity.EvaluationProgress.Single(x => x.Type == type);

            progressEntity.DateModified = DateTime.UtcNow;
            progressEntity.ModifiedBy = userId;
            progressEntity.Progress = status;
            progressEntity.Message = message;

            var affectedRecords = _context.SaveChanges();
            return affectedRecords == 1;
        }

        public bool TestExists(string name)
            => _context.AnalysisTests
                       .Where(t => t.Name.ToLower().Equals(name.ToLower()))
                       .Any();
    }
}
