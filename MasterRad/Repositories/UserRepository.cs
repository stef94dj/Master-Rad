using MasterRad.Entities;
using MasterRad.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MasterRad.Repositories
{
    public interface IUserRepository
    {
        IEnumerable<Guid> UnmappedIds(IEnumerable<Guid> userIds);
        bool CreateMapping(Guid id, string sqlUsername, string sqlPass, Guid currentUserId);
    }

    public class UserRepository : IUserRepository
    {
        private readonly Context _context;

        public UserRepository(Context context)
        {
            _context = context;
        }

        public IEnumerable<Guid> UnmappedIds(IEnumerable<Guid> userIds)
        {
            var mappedIds = _context.AzureSqlUserMap
                                    .Where(x => userIds.Contains(x.AzureId))
                                    .Select(x => x.AzureId);

            return userIds.Except(mappedIds);
        }

        public bool CreateMapping(Guid id, string sqlUsername, string sqlPass, Guid currentUserId)
        {
            var entity = new AzureSqlUserMapEntity() //AutoMapper
            {
                AzureId =  id,
                SqlUsername = sqlUsername,
                SqlPassword = sqlPass,
                DateCreated = DateTime.UtcNow,
                CreatedBy = currentUserId,
            };

            _context.AzureSqlUserMap.Add(entity);
            return _context.SaveChanges() == 1;

            throw new NotImplementedException();
        }
    }
}
