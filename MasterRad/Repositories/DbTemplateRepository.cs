using MasterRad.DTOs;
using MasterRad.Entities;
using System;

namespace MasterRad.Repositories
{
    public interface IDbTemplateRepository
    {
        DbTemplateEntity Create(string templateName);
        DbTemplateEntity UpdateDescription(UpdateDescriptionRQ request);
    }

    public class DbTemplateRepository : IDbTemplateRepository
    {
        private readonly Context _context;

        public DbTemplateRepository(Context context)
        {
            _context = context;
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
    }
}
