using MasterRad.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Repositories
{
    public interface IDbTemplateRepository
    {
        void Insert(string sqlScript, string dbName);
    }

    public class DbTemplateRepository : IDbTemplateRepository
    {
        private readonly Context _context;

        public DbTemplateRepository(Context context)
        {
            _context = context;
        }

        public void Insert(string sqlScript, string dbName)
        {
            var dbTemplateEntity = new DbTemplateEntity()
            {
                SqlScript = sqlScript,
                NameOnServer = dbName,
                DateCreated = DateTime.UtcNow,
                CreatedBy = "Current user - NOT IMPLEMENTED",
            };

            _context.DbTemplates.Add(dbTemplateEntity);
            _context.SaveChanges();
        }
    }
}
