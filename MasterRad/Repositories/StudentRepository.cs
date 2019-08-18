﻿using MasterRad.DTOs;
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
        IEnumerable<StudentEntity> GetAssignedSynthesis(int testId);
        IEnumerable<StudentEntity> GetAssignedAnalysis(int testId);
        IEnumerable<SynthesisTestStudentEntity> AssignSynthesisTest(List<int> studentIds, int testId);
        IEnumerable<AnalysisTestStudentEntity> AssignAnalysisTest(List<int> studentIds, int testId);

    }

    public class StudentRepository : IStudentRepository
    {
        private readonly Context _context;

        public StudentRepository(Context context)
        {
            _context = context;
        }

        public IEnumerable<StudentEntity> GetAssignedSynthesis(int testId)
        {
            return _context.SynthesysTestStudents
                           .Where(sts => sts.SynthesisTestId == testId)
                           .Include(sts => sts.Student)
                           .Select(sts => sts.Student);
        }

        public IEnumerable<StudentEntity> GetAssignedAnalysis(int testId)
        {
            return _context.AnalysisTestStudents
                           .Where(sts => sts.AnalysisTestId == testId)
                           .Include(sts => sts.Student)
                           .Select(sts => sts.Student);
        }

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

        public IEnumerable<SynthesisTestStudentEntity> AssignSynthesisTest(List<int> studentIds, int testId)
        {
            var stsEntities = studentIds.Select(studentId => new SynthesisTestStudentEntity()
            {
                SynthesisTestId = testId,
                StudentId = studentId,
                NameOnServer = "Not implemented",
                DateCreated = DateTime.UtcNow,
                CreatedBy = "Current user - NOT IMPLEMENTED"
            });

            _context.SynthesysTestStudents.AddRange(stsEntities);
            _context.SaveChanges();

            return stsEntities;
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
    }
}