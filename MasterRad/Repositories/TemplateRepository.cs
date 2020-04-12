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
        List<TemplateEntity> Get();
        TemplateEntity GetWithTaks(int id);
        bool Create(string templateName, string dbName, Guid userId);
        bool UpdateDescription(UpdateDescriptionRQ request, Guid userId);
        bool UpdateName(UpdateNameRQ request, Guid userId);
        bool DatabaseRegisteredAsTemplate(string name);
        bool TemplateExists(string templateName);
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
                       .Single();

        public TemplateEntity GetWithTaks(int id)
            => _context.Templates
                       .Include(t => t.Tasks)
                       .Where(x => x.Id == id)
                       .Single();

        public List<TemplateEntity> Get()
            => _context.Templates
                       .Include(x => x.Tasks)
                       .OrderByDescending(t => t.DateCreated)
                       .ToList();

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

        public bool TemplateExists(string templateName)
            => _context.Templates
                       .Where(t => t.Name.ToLower().Equals(templateName.ToLower()))
                       .Any();

        public bool DatabaseRegisteredAsTemplate(string dbName)
            => _context.Templates
                       .Where(t => t.NameOnServer.ToLower().Equals(dbName.ToLower()))
                       .Any();
    }
}
