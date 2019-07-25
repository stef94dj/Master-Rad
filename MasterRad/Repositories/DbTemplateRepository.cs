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
        DbTemplateEntity Get(int id);
        List<DbTemplateEntity> Get();
        DbTemplateEntity Create(string templateName);
        DbTemplateEntity UpdateDescription(UpdateDescriptionRQ request);
        DbTemplateEntity UpdateName(UpdateNameRQ request);
        DbTemplateEntity UpdateSqlScript(SetSqlScriptRQ request, string name);
        bool DatabaseExists(string name);
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
            return _context.DbTemplates //.Include(x => x.Tasks)
                .OrderByDescending(t => t.DateCreated)
                .ToList();
        }

        public DbTemplateEntity Create(string templateName)
        {
            var dbTemplateEntity = new DbTemplateEntity() //AutoMapper
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

        public DbTemplateEntity UpdateSqlScript(SetSqlScriptRQ request, string name)
        {
            var dbTemplateEntity = new DbTemplateEntity() //AutoMapper
            {
                Id = request.Id,
                TimeStamp = request.TimeStamp,
                SqlScript = request.SqlScript,
                NameOnServer = name,
                DateModified = DateTime.UtcNow,
                ModifiedBy = "Current user - NOT IMPLEMENTED",
            };

            _context.DbTemplates.Attach(dbTemplateEntity);
            _context.Entry(dbTemplateEntity).Property(x => x.SqlScript).IsModified = true;
            _context.Entry(dbTemplateEntity).Property(x => x.NameOnServer).IsModified = true;
            _context.SaveChanges();

            return dbTemplateEntity;
        }

        public bool DatabaseExists(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            return _context.DbTemplates
                .Where(t => t.NameOnServer.ToLower().Equals(name.ToLower()))
                .Any();
        }
    }
}
