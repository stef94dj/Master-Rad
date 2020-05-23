using MasterRad.DTO.RQ;
using MasterRad.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MasterRad.Repositories
{
    public interface ITemplateRepository
    {
        TemplateEntity Get(int id);
        List<TemplateEntity> GetPaginated(SearchPaginatedRQ searchRQ, out int pageCnt, out int pageNo);
        IEnumerable<TemplateEntity> GetPublic();
        TemplateEntity GetWithTaks(int id);
        bool Create(string templateName, string dbName, Guid userId);
        bool UpdateDescription(UpdateDescriptionRQ request, Guid userId);
        bool UpdateName(UpdateNameRQ request, Guid userId);
        bool UpdateIsPublic(UpdateIsPublicRQ request, Guid userId);
        bool DatabaseRegisteredAsTemplate(string name);
        bool TemplateExists(string templateName);

        bool DeleteTemplate(int id, byte[] timeStamp);
    }

    public class TemplateRepository : ITemplateRepository
    {
        private readonly Context _context;

        public TemplateRepository(Context context)
        {
            _context = context;
        }

        public TemplateEntity Get(int id)
            => _context.Templates
                       .Where(x => x.Id == id)
                       .AsNoTracking()
                       .Single();

        public TemplateEntity GetWithTaks(int id)
            => _context.Templates
                       .Include(t => t.Tasks)
                       .Where(x => x.Id == id)
                       .Single();

        public List<TemplateEntity> GetPaginated(SearchPaginatedRQ request, out int pageCnt, out int pageNo)
        {
            var qry = _context.Templates
                              .Include(x => x.Tasks)
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

        public IEnumerable<TemplateEntity> GetPublic()
            => _context.Templates
                       .Where(t => t.IsPublic)
                       .AsNoTracking();

        public bool Create(string templateName, string dbName, Guid userId)
        {
            var templateEntity = new TemplateEntity() //AutoMapper
            {
                Name = templateName,
                NameOnServer = dbName,
                DateCreated = DateTime.UtcNow,
                CreatedBy = userId,
            };

            _context.Templates.Add(templateEntity);
            return _context.SaveChanges() == 1;
        }

        public bool UpdateDescription(UpdateDescriptionRQ request, Guid userId)
        {
            var templateEntity = new TemplateEntity() //AutoMapper
            {
                Id = request.Id,
                TimeStamp = request.TimeStamp,
                ModelDescription = request.Description,
                DateModified = DateTime.UtcNow,
                ModifiedBy = userId,
            };

            _context.Templates.Attach(templateEntity);
            _context.Entry(templateEntity).Property(x => x.ModelDescription).IsModified = true;
            _context.Entry(templateEntity).Property(x => x.DateModified).IsModified = true;
            _context.Entry(templateEntity).Property(x => x.ModifiedBy).IsModified = true;

            return _context.SaveChanges() == 1;
        }

        public bool UpdateName(UpdateNameRQ request, Guid userId)
        {
            var templateEntity = new TemplateEntity() //AutoMapper
            {
                Id = request.Id,
                TimeStamp = request.TimeStamp,
                Name = request.Name,
                DateModified = DateTime.UtcNow,
                ModifiedBy = userId,
            };

            _context.Templates.Attach(templateEntity);
            _context.Entry(templateEntity).Property(x => x.Name).IsModified = true;
            _context.Entry(templateEntity).Property(x => x.DateModified).IsModified = true;
            _context.Entry(templateEntity).Property(x => x.ModifiedBy).IsModified = true;

            return _context.SaveChanges() == 1;
        }

        public bool UpdateIsPublic(UpdateIsPublicRQ request, Guid userId)
        {
            var templateEntity = new TemplateEntity() //AutoMapper
            {
                Id = request.Id,
                TimeStamp = request.TimeStamp,
                IsPublic = request.IsPublic,
                DateModified = DateTime.UtcNow,
                ModifiedBy = userId,
            };

            _context.Templates.Attach(templateEntity);
            _context.Entry(templateEntity).Property(x => x.IsPublic).IsModified = true;
            _context.Entry(templateEntity).Property(x => x.DateModified).IsModified = true;
            _context.Entry(templateEntity).Property(x => x.ModifiedBy).IsModified = true;

            return _context.SaveChanges() == 1;
        }

        public bool TemplateExists(string templateName)
            => _context.Templates
                       .Where(t => t.Name.ToLower().Equals(templateName.ToLower()))
                       .Any();

        public bool DatabaseRegisteredAsTemplate(string dbName)
            => _context.Templates
                       .Where(t => t.NameOnServer.ToLower().Equals(dbName.ToLower()))
                       .Any();

        public bool DeleteTemplate(int id, byte[] timeStamp)
        {
            var entity = new TemplateEntity()
            {
                Id = id,
                TimeStamp = timeStamp
            };

            _context.Templates.Remove(entity);
            return _context.SaveChanges() == 1;
        }
    }
}
