using MasterRad.DTO.RQ;
using MasterRad.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MasterRad.Repositories
{
    public interface ITaskRepository
    {
        List<TaskEntity> GetPaginated(SearchPaginatedRQ request, out int pageCnt, out int pageNo);
        TaskEntity Get(int id);
        bool Create(CreateTaskRQ request, string nameOnServer, Guid userId);
        bool UpdateName(UpdateNameRQ request, Guid userId);
        bool UpdateDescription(UpdateDescriptionRQ request, Guid userId);
        TaskEntity UpdateSolution(UpdateTaskSolutionRQ request, Guid userId);
        bool TaskExists(string taskName);
        bool DatabaseRegisteredAsTask(string dbName);
    }

    public class TaskRepository : ITaskRepository
    {
        private readonly Context _context;

        public TaskRepository(Context context)
        {
            _context = context;
        }

        public List<TaskEntity> GetPaginated(SearchPaginatedRQ request, out int pageCnt, out int pageNo)
        {
            var qry = _context.Tasks
                              .Include(x => x.Template)
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
                    case "template_name":
                        qry = qry.Where(x => x.Template.Name.ToLower().Contains(filterValue));
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
                    case "template_name":
                        qry = request.SortDescending ?
                                qry.OrderByDescending(x => x.Template.Name) :
                                qry.OrderBy(x => x.Template.Name);
                        break;
                    case "date_created":
                        qry = request.SortDescending ?
                                qry.OrderByDescending(x => x.DateCreated) :
                                qry.OrderBy(x => x.DateCreated);
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

        public TaskEntity Get(int id)
            => _context.Tasks
                       .AsNoTracking()
                       .Where(x => x.Id == id)
                       .Single();

        public bool Create(CreateTaskRQ request, string nameOnServer, Guid userId)
        {
            var taskEntity = new TaskEntity() //AutoMapper
            {
                Name = request.Name,
                TemplateId = request.TemplateId,
                NameOnServer = nameOnServer,
                DateCreated = DateTime.UtcNow,
                CreatedBy = userId,
            };

            _context.Tasks.Add(taskEntity);
            return _context.SaveChanges() == 1;
        }

        public bool UpdateName(UpdateNameRQ request, Guid userId)
        {
            var taskEntity = new TaskEntity() //AutoMapper
            {
                Id = request.Id,
                TimeStamp = request.TimeStamp,
                Name = request.Name,
                DateModified = DateTime.UtcNow,
                ModifiedBy = userId,
            };

            _context.Tasks.Attach(taskEntity);
            _context.Entry(taskEntity).Property(x => x.Name).IsModified = true;
            _context.Entry(taskEntity).Property(x => x.DateModified).IsModified = true;
            _context.Entry(taskEntity).Property(x => x.ModifiedBy).IsModified = true;

            return _context.SaveChanges() == 1;
        }

        public bool UpdateDescription(UpdateDescriptionRQ request, Guid userId)
        {
            var taskEntity = new TaskEntity() //AutoMapper
            {
                Id = request.Id,
                TimeStamp = request.TimeStamp,
                Description = request.Description,
                DateModified = DateTime.UtcNow,
                ModifiedBy = userId,
            };

            _context.Tasks.Attach(taskEntity);
            _context.Entry(taskEntity).Property(x => x.Description).IsModified = true;
            _context.Entry(taskEntity).Property(x => x.DateModified).IsModified = true;
            _context.Entry(taskEntity).Property(x => x.ModifiedBy).IsModified = true;

            return _context.SaveChanges() == 1;
        }

        public TaskEntity UpdateSolution(UpdateTaskSolutionRQ request, Guid userId)
        {
            var taskEntity = new TaskEntity() //AutoMapper
            {
                Id = request.Id,
                TimeStamp = request.TimeStamp,
                SolutionSqlScript = request.SolutionSqlScript,
                DateModified = DateTime.UtcNow,
                ModifiedBy = userId,
            };

            var newColumnEntities = request.Columns.Select(cn => new SolutionColumnEntity() //AutoMapper
            {
                ColumnName = cn.Name,
                SqlType = cn.SqlType,
                TaskId = request.Id,
                DateCreated = DateTime.UtcNow,
                CreatedBy = userId
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

        public bool TaskExists(string taskName)
            => _context.Tasks
                       .Where(t => t.Name.ToLower().Equals(taskName.ToLower()))
                       .Any();

        public bool DatabaseRegisteredAsTask(string dbName)
            => _context.Tasks
                       .Where(t => t.NameOnServer.ToLower().Equals(dbName.ToLower()))
                       .Any();
    }
}
