using MasterRad.DTOs;
using MasterRad.Entities;
using MasterRad.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MasterRad.Repositories
{
    public interface IDbTemplateRepository
    {
        DbTemplateEntity Get(int id);
        List<DbTemplateEntity> Get();
        DbTemplateEntity Create(string templateName, string dbName);
        DbTemplateEntity UpdateDescription(UpdateDescriptionRQ request);
        DbTemplateEntity UpdateName(UpdateNameRQ request);
        bool DatabaseRegisteredAsTemplate(string name);
        bool TemplateExists(string templateName);
    }

    public class DbTemplateRepository : IDbTemplateRepository
    {
        private readonly Context _context;

        public DbTemplateRepository(Context context)
        {
            _context = context;
        }


        public DbTemplateEntity Get(int id)
        {
            return _context.DbTemplates.Where(x => x.Id == id).Single();
        }

        public List<DbTemplateEntity> Get()
        {
            return _context.DbTemplates
                .Include(x => x.Tasks)
                .OrderByDescending(t => t.DateCreated)
                .ToList();
        }

        public DbTemplateEntity Create(string templateName, string dbName)
        {
            var dbTemplateEntity = new DbTemplateEntity() //AutoMapper
            {
                Name = templateName,
                NameOnServer = dbName,
                DateCreated = DateTime.UtcNow,
                CreatedBy = "Current user - NOT IMPLEMENTED",
            };

            _context.DbTemplates.Add(dbTemplateEntity);
            _context.SaveChanges();

            return dbTemplateEntity;
        }

        public DbTemplateEntity UpdateDescription(UpdateDescriptionRQ request)
        {
            var dbTemplateEntity = new DbTemplateEntity() //AutoMapper
            {
                Id = request.Id,
                TimeStamp = request.TimeStamp,
                ModelDescription = request.Description,
                DateModified = DateTime.UtcNow,
                ModifiedBy = "Current user - NOT IMPLEMENTED",
            };

            _context.DbTemplates.Attach(dbTemplateEntity);
            _context.Entry(dbTemplateEntity).Property(x => x.ModelDescription).IsModified = true;
            _context.SaveChanges();

            return dbTemplateEntity;
        }

        public DbTemplateEntity UpdateName(UpdateNameRQ request)
        {
            var dbTemplateEntity = new DbTemplateEntity() //AutoMapper
            {
                Id = request.Id,
                TimeStamp = request.TimeStamp,
                Name = request.Name,
                DateModified = DateTime.UtcNow,
                ModifiedBy = "Current user - NOT IMPLEMENTED",
            };

            _context.DbTemplates.Attach(dbTemplateEntity);
            _context.Entry(dbTemplateEntity).Property(x => x.Name).IsModified = true;
            _context.SaveChanges();

            return dbTemplateEntity;
        }

        public bool TemplateExists(string templateName)
        {
            if (string.IsNullOrEmpty(templateName))
                return false;

            return _context.DbTemplates
                .Where(t => t.Name.ToLower().Equals(templateName.ToLower()))
                .Any();
        }

        public bool DatabaseRegisteredAsTemplate(string dbName)
        {
            if (string.IsNullOrEmpty(dbName))
                return false;

            return _context.DbTemplates
                .Where(t => t.NameOnServer.ToLower().Equals(dbName.ToLower()))
                .Any();
        }
    }
}
