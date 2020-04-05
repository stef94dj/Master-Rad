using MasterRad.DTO;
using MasterRad.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Services
{
    public interface ISignalR<T>
    {
        Task SendMessageAsync(string group, string method, object message);
    }

    public class SignalR<T> : ISignalR<T> where T : Hub
    {
        private readonly IHubContext<T> _hubContext;

        public SignalR
        (
            IHubContext<T> hubContext
        )
        {
            _hubContext = hubContext;
        }

        public async Task SendMessageAsync(string group, string method, object message)
        {
            await _hubContext.Clients.Group(group).SendAsync(method, message);
        }
    }
}
