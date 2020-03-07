using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class AnalysisProgressHub : Hub
{
    public async Task AssociateJob(string jobId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, jobId);
    }
}