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
    public interface ISynthesisRepository
    {
        SynthesisTestEntity Get(int testId);
        SynthesisTestEntity GetWithTaskAndTemplate(int testId);
        IEnumerable<SynthesisTestEntity> Get();
        SynthesisTestStudentEntity GetAssignment(int studentId, int testId);
        SynthesisTestStudentEntity GetAssignmentWithTaskAndTemplate(int studentId, int testId);
        IEnumerable<SynthesisTestStudentEntity> GetAssigned(int studentId);
        IEnumerable<string> GetSolutionFormat(int testId);
        bool IsAssigned(int studentId, int testId);
        bool Create(SynthesisCreateRQ request);
        void Delete(DeleteDTO request);
        bool UpdateName(UpdateNameRQ request);
        bool StatusNext(UpdateDTO request);
        bool MarkExamAsTaken(int testId, int studentId, byte[] timeStamp);
        byte[] SubmitAnswer(int testId, int studentId, byte[] timeStamp, string sqlScript);
        bool HasAnswer(int testId, int userId);
        SynthesisTestStudentEntity GetEvaluationData(int testId, int studentId);
        bool SaveProgress(SynthesisTestStudentEntity entity, bool isSecret, EvaluationProgress status, string message = null);
        IEnumerable<SynthesisTestStudentEntity> GetPapers(int testId);
    }

    public class SynthesisRepository : ISynthesisRepository
    {
        private readonly Context _context;

        public SynthesisRepository(Context context)
        {
            _context = context;
        }

        public SynthesisTestEntity Get(int testId)
            => _context.SynthesisTests
                       .Where(t => t.Id == testId)
                       .AsNoTracking()
                       .SingleOrDefault();


        public SynthesisTestEntity GetWithTaskAndTemplate(int testId)
            => _context.SynthesisTests
                       .Where(t => t.Id == testId)
                       .Include(t => t.Task)
                       .ThenInclude(t => t.Template)
                       .AsNoTracking()
                       .SingleOrDefault();

        public IEnumerable<SynthesisTestEntity> Get()
            => _context.SynthesisTests
                       .Include(st => st.Task)
                       .ThenInclude(t => t.Template)
                       .OrderByDescending(t => t.DateCreated);

        public SynthesisTestStudentEntity GetAssignment(int studentId, int testId)
            => _context.SynthesysTestStudents
                       .Where(sts => sts.StudentId == studentId && sts.SynthesisTestId == testId)
                       .AsNoTracking()
                       .SingleOrDefault();

        public SynthesisTestStudentEntity GetAssignmentWithTaskAndTemplate(int studentId, int testId)
            => _context.SynthesysTestStudents
                       .Where(sts => sts.StudentId == studentId && sts.SynthesisTestId == testId)
                       .Include(sts => sts.SynthesisTest)
                       .ThenInclude(test => test.Task)
                       .ThenInclude(task => task.Template)
                       .AsNoTracking()
                       .SingleOrDefault();

        public IEnumerable<SynthesisTestStudentEntity> GetAssigned(int studentId)
            => _context.SynthesysTestStudents
                       .Include(sts => sts.EvaluationProgress)
                       .Include(sts => sts.SynthesisTest)
                       .Where(sts => sts.StudentId == studentId);

        public IEnumerable<string> GetSolutionFormat(int testId)
            => _context.SynthesisTests
                       .Where(st => st.Id == testId)
                       .Include(st => st.Task)
                       .ThenInclude(t => t.SolutionColumns)
                       .Single()
                       .Task
                       .SolutionColumns
                       .Select(sc => sc.ColumnName);

        public bool IsAssigned(int studentId, int testId)
            => _context.SynthesysTestStudents
                       .Where(sts => sts.StudentId == studentId && sts.SynthesisTestId == testId)
                       .AsNoTracking()
                       .Any();

        public bool Create(SynthesisCreateRQ request)
        {
            var synthesisTestEntity = new SynthesisTestEntity() //AutoMapper
            {
                Name = request.Name,
                Status = TestStatus.Scheduled,
                TaskId = request.TaskId,
                DateCreated = DateTime.UtcNow,
                CreatedBy = "Current user - NOT IMPLEMENTED",
            };

            _context.SynthesisTests.Add(synthesisTestEntity);
            return _context.SaveChanges() == 1;
        }

        public void Delete(DeleteDTO request)
        {
            var synthesisTestEntity = new SynthesisTestEntity() //AutoMapper
            {
                Id = request.Id,
                TimeStamp = request.TimeStamp,
            };

            _context.SynthesisTests.Attach(synthesisTestEntity);
            _context.SynthesisTests.Remove(synthesisTestEntity);
            _context.SaveChanges();
        }

        public bool UpdateName(UpdateNameRQ request)
        {
            var synthesisTestEntity = new SynthesisTestEntity() //AutoMapper
            {
                Id = request.Id,
                TimeStamp = request.TimeStamp,
                Name = request.Name,
                DateModified = DateTime.UtcNow,
                ModifiedBy = "Current user - NOT IMPLEMENTED",
            };

            _context.SynthesisTests.Attach(synthesisTestEntity);
            _context.Entry(synthesisTestEntity).Property(x => x.Name).IsModified = true;
            _context.Entry(synthesisTestEntity).Property(x => x.DateModified).IsModified = true;
            _context.Entry(synthesisTestEntity).Property(x => x.ModifiedBy).IsModified = true;

            return _context.SaveChanges() == 1;
        }

        public bool StatusNext(UpdateDTO request)
        {
            var currentStatus = Get(request.Id).Status;

            if (currentStatus == TestStatus.Completed)
                return false;

            var synthesisTestEntity = new SynthesisTestEntity() //AutoMapper
            {
                Id = request.Id,
                TimeStamp = request.TimeStamp,
                Status = currentStatus + 1,
                DateModified = DateTime.UtcNow,
                ModifiedBy = "Current user - NOT IMPLEMENTED",
            };

            _context.SynthesisTests.Attach(synthesisTestEntity);
            _context.Entry(synthesisTestEntity).Property(x => x.Status).IsModified = true;
            _context.Entry(synthesisTestEntity).Property(x => x.DateModified).IsModified = true;
            _context.Entry(synthesisTestEntity).Property(x => x.ModifiedBy).IsModified = true;

            return _context.SaveChanges() == 1;
        }

        public bool HasAnswer(int testId, int userId)
        {
            var sqlAnswer = _context.SynthesysTestStudents
                                               .Where(sts => sts.SynthesisTestId == testId && sts.StudentId == userId)
                                               .AsNoTracking()
                                               .Single()
                                               ?.SqlScript;

            return !string.IsNullOrEmpty(sqlAnswer);
        }

        public bool MarkExamAsTaken(int testId, int studentId, byte[] timeStamp)
        {
            var entity = new SynthesisTestStudentEntity() //AutoMapper
            {
                StudentId = studentId,
                SynthesisTestId = testId,
                TimeStamp = timeStamp,
                TakenTest = true,
                DateModified = DateTime.UtcNow,
                ModifiedBy = "Current user - NOT IMPLEMENTED",
            };

            _context.SynthesysTestStudents.Attach(entity);
            _context.Entry(entity).Property(x => x.TakenTest).IsModified = true;
            _context.Entry(entity).Property(x => x.DateModified).IsModified = true;
            _context.Entry(entity).Property(x => x.ModifiedBy).IsModified = true;

            return _context.SaveChanges() == 1;
        }

        public byte[] SubmitAnswer(int testId, int studentId, byte[] timeStamp, string sqlScript)
        {

            var entity = new SynthesisTestStudentEntity() //AutoMapper
            {
                SynthesisTestId = testId,
                StudentId = studentId,
                TimeStamp = timeStamp,
                SqlScript = sqlScript,
                DateModified = DateTime.UtcNow,
                ModifiedBy = "Current user - NOT IMPLEMENTED",
            };

            _context.SynthesysTestStudents.Attach(entity);
            _context.Entry(entity).Property(x => x.SqlScript).IsModified = true;
            _context.Entry(entity).Property(x => x.DateModified).IsModified = true;
            _context.Entry(entity).Property(x => x.ModifiedBy).IsModified = true;

            return _context.SaveChanges() == 1 ? entity.TimeStamp : null;
        }

        public SynthesisTestStudentEntity GetEvaluationData(int testId, int studentId)
        =>
            _context.SynthesysTestStudents.Where(sts => sts.SynthesisTestId == testId && sts.StudentId == studentId)
                                          .Include(sts => sts.EvaluationProgress)
                                          .Include(sts => sts.SynthesisTest)
                                          .ThenInclude(st => st.Task)
                                          .ThenInclude(task => task.Template)
                                          .Include(sts => sts.SynthesisTest)
                                          .ThenInclude(st => st.Task)
                                          .ThenInclude(task => task.SolutionColumns)
                                          .Single();

        public bool SaveProgress(SynthesisTestStudentEntity entity, bool isSecret, EvaluationProgress status, string message = null)
        {
            var progressEntity = entity.EvaluationProgress.Single(x => x.IsSecretDataUsed == isSecret);

            progressEntity.DateModified = DateTime.UtcNow;
            progressEntity.ModifiedBy = "Current user - NOT IMPLEMENTED";
            progressEntity.Progress = status;
            progressEntity.Message = message;
            
            return _context.SaveChanges() == 1;
        }

        public IEnumerable<SynthesisTestStudentEntity> GetPapers(int testId)
            => _context.SynthesysTestStudents
                       .Include(sts => sts.EvaluationProgress)
                       .Include(sts => sts.Student)
                       .Where(sts => sts.SynthesisTestId == testId);
    }
}
