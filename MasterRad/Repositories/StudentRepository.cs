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
    public interface IStudentRepository
    {
        IEnumerable<StudentEntity> SearchStudents(SearchStudentRQ request);
        IEnumerable<BaseTestStudentEntity> GetAssignedSynthesis(int testId);
        IEnumerable<BaseTestStudentEntity> GetAssignedAnalysis(int testId);
        int AssignSynthesisTest(IEnumerable<KeyValuePair<int, string>> studDbNamePairs, int testId);
        int AssignAnalysisTest(IEnumerable<AnalysisAssignModel> assignModels, int testId);
        bool RemoveSynthesis(int studentId, byte[] timeStamp, int testId);
        bool RemoveAnalysis(int studentId, byte[] timeStamp, int testId);
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

        public int AssignSynthesisTest(IEnumerable<KeyValuePair<int, string>> studDbNamePairs, int testId)
        {
            var stsEntities = studDbNamePairs.Select(pair => new SynthesisTestStudentEntity()
            {
                SynthesisTestId = testId,
                StudentId = pair.Key,
                NameOnServer = pair.Value,
                DateCreated = DateTime.UtcNow,
                CreatedBy = "Current user - NOT IMPLEMENTED",
            });

            var progressEntites = stsEntities.Select(sts => new SynthesisEvaluationEntity()
            {
                IsSecretDataUsed = false,
                STS_StudentId = sts.StudentId,
                STS_SynthesisTestId = sts.SynthesisTestId,
                Progress = EvaluationProgress.NotEvaluated,
                DateCreated = DateTime.UtcNow,
                CreatedBy = "Current user - NOT IMPLEMENTED",
            }).Union(stsEntities.Select(sts => new SynthesisEvaluationEntity()
            {
                IsSecretDataUsed = true,
                STS_StudentId = sts.StudentId,
                STS_SynthesisTestId = sts.SynthesisTestId,
                Progress = EvaluationProgress.NotEvaluated,
                DateCreated = DateTime.UtcNow,
                CreatedBy = "Current user - NOT IMPLEMENTED",
            }));

            _context.SynthesysTestEvaluations.AddRange(progressEntites);
            _context.SynthesysTestStudents.AddRange(stsEntities);
            return _context.SaveChanges();
        }

        public int AssignAnalysisTest(IEnumerable<AnalysisAssignModel> assignModels, int testId)
        {
            var atsEntities = assignModels.Select(assignModel => new AnalysisTestStudentEntity()
            {
                AnalysisTestId = testId,
                StudentId = assignModel.StudentId,
                InputNameOnServer = assignModel.Database,
                StudentOutputNameOnServer = assignModel.StudentOutputTable,
                TeacherOutputNameOnServer = assignModel.TeacherOutputTable,
                DateCreated = DateTime.UtcNow,
                CreatedBy = "Current user - NOT IMPLEMENTED"
            });

            var progressEntities = atsEntities.Select(ats => new AnalysisEvaluationEntity()
            {
                ATS_AnalysisTestId = ats.AnalysisTestId,
                ATS_StudentId = ats.StudentId,
                Type = AnalysisEvaluationType.PrepareData,
                Progress = EvaluationProgress.NotEvaluated,
                DateCreated = DateTime.UtcNow,
                CreatedBy = "Current user - NOT IMPLEMENTED"

            }).Union(atsEntities.Select(ats => new AnalysisEvaluationEntity()
            {
                ATS_AnalysisTestId = ats.AnalysisTestId,
                ATS_StudentId = ats.StudentId,
                Type = AnalysisEvaluationType.FailingInput,
                Progress = EvaluationProgress.NotEvaluated,
                DateCreated = DateTime.UtcNow,
                CreatedBy = "Current user - NOT IMPLEMENTED"

            }).Union(atsEntities.Select(ats => new AnalysisEvaluationEntity()
            {
                ATS_AnalysisTestId = ats.AnalysisTestId,
                ATS_StudentId = ats.StudentId,
                Type = AnalysisEvaluationType.QueryOutput,
                Progress = EvaluationProgress.NotEvaluated,
                DateCreated = DateTime.UtcNow,
                CreatedBy = "Current user - NOT IMPLEMENTED"

            }).Union(atsEntities.Select(ats => new AnalysisEvaluationEntity()
            {
                ATS_AnalysisTestId = ats.AnalysisTestId,
                ATS_StudentId = ats.StudentId,
                Type = AnalysisEvaluationType.CorrectOutput,
                Progress = EvaluationProgress.NotEvaluated,
                DateCreated = DateTime.UtcNow,
                CreatedBy = "Current user - NOT IMPLEMENTED"

            }))));

            _context.AnalysisTestEvaluations.AddRange(progressEntities);
            _context.AnalysisTestStudents.AddRange(atsEntities);
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

        public bool RemoveSynthesis(int studentId, byte[] timeStamp, int testId)
        {
            var stsEntity = new SynthesisTestStudentEntity()
            {
                StudentId = studentId,
                SynthesisTestId = testId,
                TimeStamp = timeStamp
            };

            _context.SynthesysTestStudents.Attach(stsEntity);
            _context.SynthesysTestStudents.Remove(stsEntity);
            return _context.SaveChanges() == 1;
        }

        public bool RemoveAnalysis(int studentId, byte[] timeStamp, int testId)
        {
            var atsEntity = new AnalysisTestStudentEntity()
            {
                StudentId = studentId,
                AnalysisTestId = testId,
                TimeStamp = timeStamp
            };

            _context.AnalysisTestStudents.Attach(atsEntity);
            _context.AnalysisTestStudents.Remove(atsEntity);
            return _context.SaveChanges() == 1;
        }
    }
}
