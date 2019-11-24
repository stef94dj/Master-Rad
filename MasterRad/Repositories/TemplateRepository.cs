using MasterRad.DTOs;
using MasterRad.Entities;
using MasterRad.Models.DTOs;
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
        TemplateEntity Create(string templateName, string dbName);
        TemplateEntity UpdateDescription(UpdateDescriptionRQ request);
        TemplateEntity UpdateName(UpdateNameRQ request);
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

        public TemplateEntity Create(string templateName, string dbName)
        {
            var templateEntity = new TemplateEntity() //AutoMapper
            {
                Name = templateName,
                NameOnServer = dbName,
                DateCreated = DateTime.UtcNow,
                CreatedBy = "Current user - NOT IMPLEMENTED",
            };

            _context.Templates.Add(templateEntity);
            _context.SaveChanges();

            return templateEntity;
        }

        public TemplateEntity UpdateDescription(UpdateDescriptionRQ request)
        {
            var templateEntity = new TemplateEntity() //AutoMapper
            {
                Id = request.Id,
                TimeStamp = request.TimeStamp,
                ModelDescription = request.Description,
                DateModified = DateTime.UtcNow,
                ModifiedBy = "Current user - NOT IMPLEMENTED",
            };

            _context.Templates.Attach(templateEntity);
            _context.Entry(templateEntity).Property(x => x.ModelDescription).IsModified = true;
            _context.Entry(templateEntity).Property(x => x.DateModified).IsModified = true;
            _context.Entry(templateEntity).Property(x => x.ModifiedBy).IsModified = true;

            _context.SaveChanges();
            return templateEntity;
        }

        public TemplateEntity UpdateName(UpdateNameRQ request)
        {
            var templateEntity = new TemplateEntity() //AutoMapper
            {
                Id = request.Id,
                TimeStamp = request.TimeStamp,
                Name = request.Name,
                DateModified = DateTime.UtcNow,
                ModifiedBy = "Current user - NOT IMPLEMENTED",
            };

            _context.Templates.Attach(templateEntity);
            _context.Entry(templateEntity).Property(x => x.Name).IsModified = true;
            _context.Entry(templateEntity).Property(x => x.DateModified).IsModified = true;
            _context.Entry(templateEntity).Property(x => x.ModifiedBy).IsModified = true;

            _context.SaveChanges();
            return templateEntity;
        }

        public bool TemplateExists(string templateName)
        {
            if (string.IsNullOrEmpty(templateName))
                return false;

            return _context.Templates
                .Where(t => t.Name.ToLower().Equals(templateName.ToLower()))
                .Any();
        }

        public bool DatabaseRegisteredAsTemplate(string dbName)
        {
            if (string.IsNullOrEmpty(dbName))
                return false;

            return _context.Templates
                .Where(t => t.NameOnServer.ToLower().Equals(dbName.ToLower()))
                .Any();
        }
    }
}
