using MasterRad.DTOs;
using MasterRad.Entities;
using MasterRad.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MasterRad.Repositories
{
    public interface IStudentRepository
    {
        IEnumerable<StudentEntity> SearchStudents(SearchStudentRQ request);
        IEnumerable<BaseTestStudentEntity> GetAssignedSynthesis(int testId);
        IEnumerable<BaseTestStudentEntity> GetAssignedAnalysis(int testId);
        int AssignSynthesisTest(IEnumerable<KeyValuePair<int, string>> studDbNamePairs, int testId);
        IEnumerable<AnalysisTestStudentEntity> AssignAnalysisTest(List<int> studentIds, int testId);
        void RemoveSynthesis(int studentId, byte[] timeStamp, int testId);
        void RemoveAnalysis(int studentId, byte[] timeStamp, int testId);
    }

    public class StudentRepository : IStudentRepository
    {
        private readonly Context _context;

        public StudentRepository(Context context)
        {
            _context = context;
        }

        public IEnumerable<BaseTestStudentEntity> GetAssignedSynthesis(int testId)
            => _context.SynthesysTestStudents
                       .Where(sts => sts.SynthesisTestId == testId)
                       .Include(sts => sts.Student);

        public IEnumerable<BaseTestStudentEntity> GetAssignedAnalysis(int testId)
            => _context.AnalysisTestStudents
                       .Where(sts => sts.AnalysisTestId == testId)
                       .Include(sts => sts.Student);

        public IEnumerable<AnalysisTestStudentEntity> AssignAnalysisTest(List<int> studentIds, int testId)
        {
            var atsEntities = studentIds.Select(studentId => new AnalysisTestStudentEntity()
            {
                AnalysisTestId = testId,
                StudentId = studentId,
                NameOnServer = "Not implemented",
                DateCreated = DateTime.UtcNow,
                CreatedBy = "Current user - NOT IMPLEMENTED"
            });

            _context.AnalysisTestStudents.AddRange(atsEntities);
            _context.SaveChanges();

            return atsEntities;
            throw new NotImplementedException();
        }

        public int AssignSynthesisTest(IEnumerable<KeyValuePair<int, string>> studDbNamePairs, int testId)
        {
            var stsEntities = studDbNamePairs.Select(pair => new SynthesisTestStudentEntity()
            {
                SynthesisTestId = testId,
                StudentId = pair.Key,
                NameOnServer = pair.Value,
                DateCreated = DateTime.UtcNow,
                CreatedBy = "Current user - NOT IMPLEMENTED"
            });

            _context.SynthesysTestStudents.AddRange(stsEntities);
            return _context.SaveChanges();
        }

        public IEnumerable<StudentEntity> SearchStudents(SearchStudentRQ rq)
        {
            IQueryable<StudentEntity> qry = _context.Students;

            if (rq.ExcludeIds != null && rq.ExcludeIds.Any())
                qry = qry.Where(s => !rq.ExcludeIds.Contains(s.Id));

            if (!string.IsNullOrEmpty(rq.FirstName))
                qry = qry.Where(s => !string.IsNullOrEmpty(s.FirstName) && s.FirstName.ToLower().Contains(rq.FirstName.ToLower()));

            if (!string.IsNullOrEmpty(rq.LastName))
                qry = qry.Where(s => !string.IsNullOrEmpty(s.LastName) && s.LastName.ToLower().Contains(rq.LastName.ToLower()));

            if (!string.IsNullOrEmpty(rq.Email))
                qry = qry.Where(s => !string.IsNullOrEmpty(s.Email) && s.Email.ToLower().Contains(rq.Email.ToLower()));

            return qry.AsNoTracking().ToList();
        }

        public void RemoveSynthesis(int studentId, byte[] timeStamp, int testId)
        {
            var stsEntity = new SynthesisTestStudentEntity()
            {
                StudentId = studentId,
                SynthesisTestId = testId,
                TimeStamp = timeStamp
            };

            _context.SynthesysTestStudents.Attach(stsEntity);
            _context.SynthesysTestStudents.Remove(stsEntity);
            _context.SaveChanges();
        }

        public void RemoveAnalysis(int studentId, byte[] timeStamp, int testId)
        {
            throw new NotImplementedException();
        }
    }
}
