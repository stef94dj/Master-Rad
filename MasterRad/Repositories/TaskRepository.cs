using MasterRad.Entities;
using MasterRad.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MasterRad.Repositories
{
    public interface ITaskRepository
    {
        List<TaskEntity> Get();
        TaskEntity Create(CreateTaskRQ request);
        TaskEntity UpdateName(UpdateNameRQ request);
        TaskEntity UpdateDescription(UpdateDescriptionRQ request);
        TaskEntity UpdateTemplate(UpdateTaskTemplateRQ request);
    }

    public class TaskRepository : ITaskRepository
    {
        private readonly Context _context;

        public TaskRepository(Context context)
        {
            _context = context;
        }

        public TaskEntity Create(CreateTaskRQ request)
        {
            var taskEntity = new TaskEntity() //AutoMapper
            {
                Name = request.Name,
                DbTemplateId = request.TemplateId,
                DateCreated = DateTime.UtcNow,
                CreatedBy = "Current user - NOT IMPLEMENTED",
            };

            _context.Tasks.Add(taskEntity);
            _context.SaveChanges();

            return taskEntity;
        }

        public List<TaskEntity> Get()
        {
            return _context.Tasks.Include(x => x.Template)
                .OrderByDescending(t => t.DateCreated)
                .ToList();
        }

        public TaskEntity UpdateDescription(UpdateDescriptionRQ request)
        {
            var taskEntity = new TaskEntity() //AutoMapper
            {
                Id = request.Id,
                TimeStamp = request.TimeStamp,
                Description = request.Description,
                DateModified = DateTime.UtcNow,
                ModifiedBy = "Current user - NOT IMPLEMENTED",
            };

            _context.Tasks.Attach(taskEntity);
            _context.Entry(taskEntity).Property(x => x.Description).IsModified = true;
            _context.SaveChanges();

            return taskEntity;
        }

        public TaskEntity UpdateName(UpdateNameRQ request)
        {
            var taskEntity = new TaskEntity() //AutoMapper
            {
                Id = request.Id,
                TimeStamp = request.TimeStamp,
                Name = request.Name,
                DateModified = DateTime.UtcNow,
                ModifiedBy = "Current user - NOT IMPLEMENTED",
            };

            _context.Tasks.Attach(taskEntity);
            _context.Entry(taskEntity).Property(x => x.Name).IsModified = true;
            _context.SaveChanges();

            return taskEntity;
        }

        public TaskEntity UpdateTemplate(UpdateTaskTemplateRQ request)
        {
            var taskEntity = new TaskEntity() //AutoMapper
            {
                Id = request.Id,
                TimeStamp = request.TimeStamp,
                DbTemplateId = request.TemplateId,
                DateModified = DateTime.UtcNow,
                ModifiedBy = "Current user - NOT IMPLEMENTED",
            };

            _context.Tasks.Attach(taskEntity);
            _context.Entry(taskEntity).Property(x => x.DbTemplateId).IsModified = true;
            _context.SaveChanges();

            return taskEntity;
        }
    }
}
