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
    }

    public class StudentRepository : IStudentRepository
    {
        private readonly Context _context;

        public StudentRepository(Context context)
        {
            _context = context;
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
