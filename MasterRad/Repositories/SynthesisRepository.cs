using MasterRad.DTOs;
using MasterRad.Entities;
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
        SynthesisTestEntity GetWithTask(int testId);
        IEnumerable<SynthesisTestEntity> Get();
        SynthesisTestStudentEntity GetAssignment(int studentId, int testId);
        SynthesisTestStudentEntity GetAssignmentWithTaskAndTemplate(int studentId, int testId);
        IEnumerable<SynthesisTestStudentEntity> GetAssigned(int studentId);
        IEnumerable<string> GetSolutionFormat(int testId);
        bool IsAssigned(int studentId, int testId);
        SynthesisTestEntity Create(SynthesisCreateRQ request);
        void Delete(DeleteDTO request);
        SynthesisTestEntity UpdateName(UpdateNameRQ request);
        SynthesisTestEntity StatusNext(UpdateDTO request);
        SynthesisPaperEntity SubmitAnswer(int testId, int studentId, string sqlScript);
        SynthesisPaperEntity UpdateAnswer(int synthesisPaperId, byte[] synthesisPaperTimeStamp, string sqlScript);
        bool HasAnswer(int testId, int userId);
    }

    public class SynthesisRepository : ISynthesisRepository
    {
        private readonly Context _context;

        public SynthesisRepository(Context context)
        {
            _context = context;
        }

        public SynthesisTestEntity Get(int testId)
        {
            return _context.SynthesisTests
                        .Where(t => t.Id == testId)
                        .AsNoTracking()
                        .SingleOrDefault();
        }

        public SynthesisTestEntity GetWithTask(int testId)
        {
            return _context.SynthesisTests
                        .Where(t => t.Id == testId)
                        .Include(t => t.Task)
                        .AsNoTracking()
                        .SingleOrDefault();
        }

        public IEnumerable<SynthesisTestEntity> Get()
        {
            return _context.SynthesisTests
                           .Include(st => st.Task)
                           .ThenInclude(t => t.Template)
                           .OrderByDescending(t => t.DateCreated);
        }
        public SynthesisTestStudentEntity GetAssignment(int studentId, int testId)
        {
            return _context.SynthesysTestStudents
                           .Where(sts => sts.StudentId == studentId && sts.SynthesisTestId == testId)
                           .AsNoTracking()
                           .SingleOrDefault();
        }
        public SynthesisTestStudentEntity GetAssignmentWithTaskAndTemplate(int studentId, int testId)
        {
            return _context.SynthesysTestStudents
                           .Where(sts => sts.StudentId == studentId && sts.SynthesisTestId == testId)
                           .Include(sts => sts.SynthesisPaper)
                           .Include(sts => sts.SynthesisTest)
                           .ThenInclude(test => test.Task)
                           .ThenInclude(task => task.Template)
                           .SingleOrDefault();
        }

        public IEnumerable<SynthesisTestStudentEntity> GetAssigned(int studentId)
        {
            return _context.SynthesysTestStudents
                           .Include(sts => sts.SynthesisTest)
                           .Where(sts => sts.StudentId == studentId);
            //.OrderByDescending(sts => sts.DateCreated);
        }

        public IEnumerable<string> GetSolutionFormat(int testId)
        {
            return _context.SynthesisTests
                           .Where(st => st.Id == testId)
                           .Include(st => st.Task)
                           .ThenInclude(t => t.SolutionColumns)
                           .Single()
                           .Task
                           .SolutionColumns
                           .Select(sc => sc.ColumnName);
        }

        public bool IsAssigned(int studentId, int testId)
        {
            return _context.SynthesysTestStudents
                            .Where(sts => sts.StudentId == studentId && sts.SynthesisTestId == testId)
                            .AsNoTracking()
                            .Any();
        }

        public SynthesisTestEntity Create(SynthesisCreateRQ request)
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
            _context.SaveChanges();
            return synthesisTestEntity;
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

        public SynthesisTestEntity UpdateName(UpdateNameRQ request)
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

            _context.SaveChanges();
            return synthesisTestEntity;
        }

        public SynthesisTestEntity StatusNext(UpdateDTO request)
        {
            var currentStatus = Get(request.Id).Status;

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

            _context.SaveChanges();
            return synthesisTestEntity;
        }

        public bool HasAnswer(int testId, int userId)
        {
            var sqlAnswer = _context.SynthesysTestStudents
                                               .Where(sts => sts.SynthesisTestId == testId && sts.StudentId == userId)
                                               .Include(sts => sts.SynthesisPaper)
                                               .AsNoTracking()
                                               .Single()
                                               .SynthesisPaper
                                               ?.SqlScript;

            return !string.IsNullOrEmpty(sqlAnswer);
        }

        public SynthesisPaperEntity SubmitAnswer(int testId, int studentId, string sqlScript)
        {

            var synthesisPaperEntity = new SynthesisPaperEntity() //AutoMapper
            {
                STS_SynthesisTestId = testId,
                STS_StudentId = studentId,
                SqlScript = sqlScript,
                DateCreated = DateTime.UtcNow,
                CreatedBy = "Current user - NOT IMPLEMENTED",
            };

            _context.SynthesisPapers.Add(synthesisPaperEntity);
            _context.SaveChanges();

            return synthesisPaperEntity;
        }

        public SynthesisPaperEntity UpdateAnswer(int synthesisPaperId, byte[] synthesisPaperTimeStamp, string sqlScript)
        {
            var synthesisPaperEntity = new SynthesisPaperEntity() //AutoMapper
            {
                Id = synthesisPaperId,
                TimeStamp = synthesisPaperTimeStamp,
                SqlScript = sqlScript,
                DateModified = DateTime.UtcNow,
                ModifiedBy = "Current user - NOT IMPLEMENTED",
            };

            _context.SynthesisPapers.Attach(synthesisPaperEntity);
            _context.Entry(synthesisPaperEntity).Property(x => x.SqlScript).IsModified = true;
            _context.Entry(synthesisPaperEntity).Property(x => x.ModifiedBy).IsModified = true;
            _context.Entry(synthesisPaperEntity).Property(x => x.DateModified).IsModified = true;

            _context.SaveChanges();
            return synthesisPaperEntity;
        }
    }
}
