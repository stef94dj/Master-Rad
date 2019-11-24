using MasterRad.DTOs;
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
        TaskEntity Get(int id);
        TaskEntity Create(CreateTaskRQ request, string nameOnServer);
        TaskEntity UpdateName(UpdateNameRQ request);
        TaskEntity UpdateDescription(UpdateDescriptionRQ request);
        TaskEntity UpdateSolution(UpdateTaskSolutionRQ request);
    }

    public class TaskRepository : ITaskRepository
    {
        private readonly Context _context;

        public TaskRepository(Context context)
        {
            _context = context;
        }

        public TaskEntity Create(CreateTaskRQ request, string nameOnServer)
        {
            var taskEntity = new TaskEntity() //AutoMapper
            {
                Name = request.Name,
                TemplateId = request.TemplateId,
                NameOnServer = nameOnServer,
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

        public TaskEntity Get(int id)
        {
            return _context.Tasks.AsNoTracking().Where(x => x.Id == id).Single();
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
            _context.Entry(taskEntity).Property(x => x.DateModified).IsModified = true;
            _context.Entry(taskEntity).Property(x => x.ModifiedBy).IsModified = true;

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
            _context.Entry(taskEntity).Property(x => x.DateModified).IsModified = true;
            _context.Entry(taskEntity).Property(x => x.ModifiedBy).IsModified = true;

            _context.SaveChanges();
            return taskEntity;
        }

        public TaskEntity UpdateSolution(UpdateTaskSolutionRQ request)
        {
            var taskEntity = new TaskEntity() //AutoMapper
            {
                Id = request.Id,
                TimeStamp = request.TimeStamp,
                SolutionSqlScript = request.SolutionSqlScript,
                DateModified = DateTime.UtcNow,
                ModifiedBy = "Current user - NOT IMPLEMENTED",
            };

            var newColumnEntities = request.ColumnNames.Select(cn => new SolutionColumnEntity() //AutoMapper
            {
                ColumnName = cn,
                TaskId = request.Id,
                DateCreated = DateTime.UtcNow,
                CreatedBy = "Current user - NOT IMPLEMENTED"
            });

            _context.Tasks.Attach(taskEntity);
            _context.Entry(taskEntity).Property(x => x.SolutionSqlScript).IsModified = true;
            _context.Entry(taskEntity).Property(x => x.DateModified).IsModified = true;
            _context.Entry(taskEntity).Property(x => x.ModifiedBy).IsModified = true;

            _context.Tasks.Where(t => t.Id == request.Id)
                          .Include(i => i.SolutionColumns)
                          .Single()
                          .SolutionColumns.Clear();

            _context.SolutionColums.AddRange(newColumnEntities);

            _context.SaveChanges();
            return taskEntity;
        }
    }
}
