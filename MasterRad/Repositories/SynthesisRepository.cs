using MasterRad.DTO;
using MasterRad.DTO.RQ;
using MasterRad.Entities;
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

        IEnumerable<SynthesisTestEntity> GetPaginated(SearchPaginatedRQ request, out int pageCnt, out int pageNo);
        SynthesisTestStudentEntity GetAssignment(Guid studentId, int testId);
        SynthesisTestStudentEntity GetAssignmentWithTaskAndTemplate(Guid studentId, int testId);
        IEnumerable<SynthesisTestStudentEntity> GetAssigned(Guid studentId);
        IEnumerable<string> GetSolutionFormat(int testId);
        bool IsAssigned(Guid studentId, int testId);
        bool Create(SynthesisCreateRQ request, Guid userId);
        void Delete(DeleteDTO request);
        bool UpdateName(UpdateNameRQ request, Guid userId);
        bool StatusNext(UpdateDTO request, Guid userId);
        bool MarkExamAsTaken(int testId, Guid studentId, ref byte[] timeStamp);
        byte[] SubmitAnswer(int testId, Guid studentId, byte[] timeStamp, string sqlScript);
        bool HasAnswer(int testId, Guid userId);
        SynthesisTestStudentEntity GetEvaluationData(int testId, Guid studentId);
        bool SaveProgress(SynthesisTestStudentEntity entity, bool isSecret, EvaluationProgress status, Guid userId, string message = null);
        bool TestExists(string name);
        IEnumerable<SynthesisTestStudentEntity> GetPapers(int testId);
        int TaskChildCount(int taskId);
        int AssignedStudentsCount(int testId);
        bool DeleteTest(int id, byte[] timeStamp);
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

        public IEnumerable<SynthesisTestEntity> GetPaginated(SearchPaginatedRQ request, out int pageCnt, out int pageNo)
        {
            var qry = _context.SynthesisTests
                              .Include(st => st.Task)
                              .ThenInclude(t => t.Template)
                              .AsNoTracking();

            #region Filter
            foreach (var filter in request.Filters)
            {
                var filterValue = filter.Value.ToLower();
                switch (filter.Key)
                {
                    case "name":
                        qry = qry.Where(x => x.Name.ToLower().Contains(filterValue));
                        break;
                    case "task_name":
                        qry = qry.Where(x => x.Task.Name.ToLower().Contains(filterValue));
                        break;
                    case "template_name":
                        qry = qry.Where(x => x.Task.Template.Name.ToLower().Contains(filterValue));
                        break;
                    case "test_status":
                        var statusId = int.Parse(filter.Value);
                        if (statusId > 0)
                            qry = qry.Where(x => x.Status == (TestStatus)statusId);
                        break;
                }
            }
            #endregion

            #region Page_Count_and_Number
            pageNo = request.Page > 0 ? request.Page : 1;
            pageCnt = 1;
            if (request.PageSize > 0)
            {
                var total = qry.Count();
                pageCnt = total / request.PageSize;
                if (total % request.PageSize > 0)
                    pageCnt++;
            }
            #endregion

            #region Sort
            if (string.IsNullOrEmpty(request.SortBy))
            {
                qry = qry.OrderBy(x => x.DateCreated);
            }
            else
            {
                switch (request.SortBy)
                {
                    case "name":
                        qry = request.SortDescending ?
                                qry.OrderByDescending(x => x.Name) :
                                qry.OrderBy(x => x.Name);
                        break;
                    case "task_name":
                        qry = request.SortDescending ?
                                qry.OrderByDescending(x => x.Task.Name) :
                                qry.OrderBy(x => x.Task.Name);
                        break;
                    case "template_name":
                        qry = request.SortDescending ?
                                qry.OrderByDescending(x => x.Task.Template.Name) :
                                qry.OrderBy(x => x.Task.Template.Name);
                        break;
                    case "date_created":
                        qry = request.SortDescending ?
                                qry.OrderByDescending(x => x.DateCreated) :
                                qry.OrderBy(x => x.DateCreated);
                        break;
                    case "test_status":
                        qry = request.SortDescending ?
                                qry.OrderByDescending(x => x.Status) :
                                qry.OrderBy(x => x.Status);
                        break;
                }
            }
            #endregion

            #region Skip
            if (request.Page > 0 && request.PageSize > 0)
            {
                qry = qry.Skip((request.Page - 1) * request.PageSize)
                         .Take(request.PageSize);
            }
            #endregion

            return qry.ToList();
        }

        public SynthesisTestEntity GetWithTaskAndTemplate(int testId)
            => _context.SynthesisTests
                       .Where(t => t.Id == testId)
                       .Include(t => t.Task)
                       .ThenInclude(t => t.Template)
                       .AsNoTracking()
                       .SingleOrDefault();

        public SynthesisTestStudentEntity GetAssignment(Guid studentId, int testId)
            => _context.SynthesysTestStudents
                       .Where(sts => sts.StudentId == studentId && sts.SynthesisTestId == testId)
                       .AsNoTracking()
                       .SingleOrDefault();

        public SynthesisTestStudentEntity GetAssignmentWithTaskAndTemplate(Guid studentId, int testId)
            => _context.SynthesysTestStudents
                       .Where(sts => sts.StudentId == studentId && sts.SynthesisTestId == testId)
                       .Include(sts => sts.SynthesisTest)
                       .ThenInclude(test => test.Task)
                       .ThenInclude(task => task.Template)
                       .AsNoTracking()
                       .SingleOrDefault();

        public IEnumerable<SynthesisTestStudentEntity> GetAssigned(Guid studentId)
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

        public bool IsAssigned(Guid studentId, int testId)
            => _context.SynthesysTestStudents
                       .Where(sts => sts.StudentId == studentId && sts.SynthesisTestId == testId)
                       .AsNoTracking()
                       .Any();

        public bool Create(SynthesisCreateRQ request, Guid userId)
        {
            var synthesisTestEntity = new SynthesisTestEntity() //AutoMapper
            {
                Name = request.Name,
                Status = TestStatus.Scheduled,
                TaskId = request.TaskId,
                DateCreated = DateTime.UtcNow,
                CreatedBy = userId,
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

        public bool UpdateName(UpdateNameRQ request, Guid userId)
        {
            var synthesisTestEntity = new SynthesisTestEntity() //AutoMapper
            {
                Id = request.Id,
                TimeStamp = request.TimeStamp,
                Name = request.Name,
                DateModified = DateTime.UtcNow,
                ModifiedBy = userId,
            };

            _context.SynthesisTests.Attach(synthesisTestEntity);
            _context.Entry(synthesisTestEntity).Property(x => x.Name).IsModified = true;
            _context.Entry(synthesisTestEntity).Property(x => x.DateModified).IsModified = true;
            _context.Entry(synthesisTestEntity).Property(x => x.ModifiedBy).IsModified = true;

            return _context.SaveChanges() == 1;
        }

        public bool StatusNext(UpdateDTO request, Guid userId)
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
                ModifiedBy = userId,
            };

            _context.SynthesisTests.Attach(synthesisTestEntity);
            _context.Entry(synthesisTestEntity).Property(x => x.Status).IsModified = true;
            _context.Entry(synthesisTestEntity).Property(x => x.DateModified).IsModified = true;
            _context.Entry(synthesisTestEntity).Property(x => x.ModifiedBy).IsModified = true;

            return _context.SaveChanges() == 1;
        }

        public bool HasAnswer(int testId, Guid userId)
        {
            var sqlAnswer = _context.SynthesysTestStudents
                                               .Where(sts => sts.SynthesisTestId == testId && sts.StudentId == userId)
                                               .AsNoTracking()
                                               .Single()
                                               ?.SqlScript;

            return !string.IsNullOrEmpty(sqlAnswer);
        }

        public bool MarkExamAsTaken(int testId, Guid studentId, ref byte[] timeStamp)
        {
            var entity = new SynthesisTestStudentEntity() //AutoMapper
            {
                StudentId = studentId,
                SynthesisTestId = testId,
                TimeStamp = timeStamp,
                TakenTest = true,
                DateModified = DateTime.UtcNow,
                ModifiedBy = studentId,
            };

            _context.SynthesysTestStudents.Attach(entity);
            _context.Entry(entity).Property(x => x.TakenTest).IsModified = true;
            _context.Entry(entity).Property(x => x.DateModified).IsModified = true;
            _context.Entry(entity).Property(x => x.ModifiedBy).IsModified = true;

            var isSuccess = _context.SaveChanges() == 1;
            if (isSuccess)
                timeStamp = entity.TimeStamp;

            return isSuccess;
        }

        public byte[] SubmitAnswer(int testId, Guid studentId, byte[] timeStamp, string sqlScript)
        {

            var entity = new SynthesisTestStudentEntity() //AutoMapper
            {
                SynthesisTestId = testId,
                StudentId = studentId,
                TimeStamp = timeStamp,
                SqlScript = sqlScript,
                DateModified = DateTime.UtcNow,
                ModifiedBy = studentId,
            };

            _context.SynthesysTestStudents.Attach(entity);
            _context.Entry(entity).Property(x => x.SqlScript).IsModified = true;
            _context.Entry(entity).Property(x => x.DateModified).IsModified = true;
            _context.Entry(entity).Property(x => x.ModifiedBy).IsModified = true;

            return _context.SaveChanges() == 1 ? entity.TimeStamp : null;
        }

        public SynthesisTestStudentEntity GetEvaluationData(int testId, Guid studentId)
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

        public bool SaveProgress(SynthesisTestStudentEntity entity, bool isSecret, EvaluationProgress status, Guid userId, string message = null)
        {
            var progressEntity = entity.EvaluationProgress.Single(x => x.IsSecretDataUsed == isSecret);

            progressEntity.DateModified = DateTime.UtcNow;
            progressEntity.ModifiedBy = userId;
            progressEntity.Progress = status;
            progressEntity.Message = message;

            return _context.SaveChanges() == 1;
        }

        public IEnumerable<SynthesisTestStudentEntity> GetPapers(int testId)
            => _context.SynthesysTestStudents
                       .Include(sts => sts.EvaluationProgress)
                       .Where(sts => sts.SynthesisTestId == testId);

        public bool TestExists(string name)
            => _context.SynthesisTests
                       .Where(t => t.Name.ToLower().Equals(name.ToLower()))
                       .Any();

        public int TaskChildCount(int taskId)
            => _context.SynthesisTests
                       .Count(t => t.TaskId == taskId);

        public int AssignedStudentsCount(int testId)
            => _context.SynthesysTestStudents
                       .Count(t => t.SynthesisTestId == testId);

        public bool DeleteTest(int id, byte[] timeStamp)
        {
            var entity = new SynthesisTestEntity()
            {
                Id = id,
                TimeStamp = timeStamp
            };

            _context.SynthesisTests.Remove(entity);
            return _context.SaveChanges() == 1;
        }
    }
}
