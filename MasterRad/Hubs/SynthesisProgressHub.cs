using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class SynthesisProgressHub : Hub
{
    public async Task AssociateJob(string jobId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, jobId);
    }
}