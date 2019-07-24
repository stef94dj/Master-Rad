using MasterRad.DTOs;
using MasterRad.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MasterRad.Repositories
{
    public interface IDbTemplateRepository
    {
        List<DbTemplateEntity> Templates();
        DbTemplateEntity Create(string templateName);
        DbTemplateEntity UpdateDescription(UpdateDescriptionRQ request);
        DbTemplateEntity UpdateName(UpdateNameRQ request);
        DbTemplateEntity UpdateSqlScript(SetSqlScriptRQ request);
        bool DatabaseExists(string name);
    }

    public class DbTemplateRepository : IDbTemplateRepository
    {
        private readonly Context _context;

        public DbTemplateRepository(Context context)
        {
            _context = context;
        }

        public List<DbTemplateEntity> Templates()
        {
            return _context.DbTemplates
                .Include(x => x.Tasks)
                .ToList();
        }

        public DbTemplateEntity Create(string templateName)
        {
            var dbTemplateEntity = new DbTemplateEntity()
            {
                Name = templateName,
                DateCreated = DateTime.UtcNow,
                CreatedBy = "Current user - NOT IMPLEMENTED",
            };

            _context.DbTemplates.Add(dbTemplateEntity);
            _context.SaveChanges();

            return dbTemplateEntity;
        }

        public DbTemplateEntity UpdateDescription(UpdateDescriptionRQ request)
        {
            var dbTemplateEntity = new DbTemplateEntity()
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
            var dbTemplateEntity = new DbTemplateEntity()
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

        public DbTemplateEntity UpdateSqlScript(SetSqlScriptRQ request)
        {
            var dbTemplateEntity = new DbTemplateEntity()
            {
                Id = request.Id,
                TimeStamp = request.TimeStamp,
                SqlScript = request.SqlScript,
                DateModified = DateTime.UtcNow,
                ModifiedBy = "Current user - NOT IMPLEMENTED",
            };

            _context.DbTemplates.Attach(dbTemplateEntity);
            _context.Entry(dbTemplateEntity).Property(x => x.SqlScript).IsModified = true;
            _context.SaveChanges();

            return dbTemplateEntity;
        }

        public bool DatabaseExists(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            return _context.DbTemplates
                .Where(t => t.Name.ToLower().Equals(name.ToLower()))
                .Any();
        }
    }
}
