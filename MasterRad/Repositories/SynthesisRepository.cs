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
        IEnumerable<SynthesisTestEntity> Get();
        IEnumerable<SynthesisTestStudentEntity> GetAssigned(int studentId);
        SynthesisTestEntity Create(SynthesisCreateRQ request);
        void Delete(DeleteDTO request);
        SynthesisTestEntity UpdateName(UpdateNameRQ request);
    }

    public class SynthesisRepository : ISynthesisRepository
    {
        private readonly Context _context;

        public SynthesisRepository(Context context)
        {
            _context = context;
        }

        public IEnumerable<SynthesisTestEntity> Get()
        {
            return _context.SynthesisTests
                           .Include(st => st.Task)
                           .ThenInclude(t => t.Template)
                           .OrderByDescending(t => t.DateCreated);
        }

        public IEnumerable<SynthesisTestStudentEntity> GetAssigned(int studentId)
        {
            return _context.SynthesysTestStudents
                           .Include(sts => sts.SynthesisTest)
                           .Where(sts => sts.StudentId == studentId);
            //.OrderByDescending(sts => sts.DateCreated);
        }

        public SynthesisTestEntity Create(SynthesisCreateRQ request)
        {
            var synthesisTestEntity = new SynthesisTestEntity() //AutoMapper
            {
                Name = request.Name,
                Status = TestStatus.Scheduled,
                TaskId = request.TaskId,
                SynthesisTestStudents = new List<SynthesisTestStudentEntity>(),
                DateCreated = DateTime.UtcNow,
                CreatedBy = "Current user - NOT IMPLEMENTED",
            };

            var synthesisTestStudentEntities = request.StudentIds
                .Select(sid => new SynthesisTestStudentEntity()
                {
                    StudentId = sid,
                    NameOnServer = "To be implemented",
                    SynthesisTest = synthesisTestEntity
                });

            synthesisTestEntity.SynthesisTestStudents.AddRange(synthesisTestStudentEntities);

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
            _context.SaveChanges();

            return synthesisTestEntity;
        }

    }
}
