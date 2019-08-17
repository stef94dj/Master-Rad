using MasterRad.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Services
{
    public interface IUser
    {
        bool IsAssigned(int synthesisTestId);
        int UserId { get; }
    }
    public class User : IUser
    {
        private readonly IConfiguration _config;
        private readonly ISynthesisRepository _synthesisRepository;

        private readonly int UserId;
        int IUser.UserId => UserId;

        public User(IConfiguration config, ISynthesisRepository synthesisRepository)
        {
            _config = config;
            _synthesisRepository = synthesisRepository;
            UserId = int.Parse(_config.GetSection("CurrentUserId").Value);
        }

        public bool IsAssigned(int synthesisTestId)
        {
            return _synthesisRepository.IsAssigned(UserId, synthesisTestId);
        }
    }
}
