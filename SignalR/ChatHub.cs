using System.Security.Claims;
using DatingApp.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DatingApp.SignalR;

[Authorize]
public class ChatHub(ChatTracker chatTracker) : Hub
{
    public override async Task OnConnectedAsync()
    {
        await chatTracker.UserConnected(GetUserId(),Context.ConnectionId);
        await Clients.Others.SendAsync("UserOnline",GetUserId());
        // When User Connect The App Give Him The List Of Connected Users

        var currentUsers = await chatTracker.GetOnlineUsers();
        await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);

    }
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await chatTracker.UserDisconnected(GetUserId(),Context.ConnectionId);
        await Clients.Others.SendAsync("UserOffLine",GetUserId());
        
        var currentUsers = await chatTracker.GetOnlineUsers();
        await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);
        
        await base.OnDisconnectedAsync(exception);
    }

    public string GetUserId()
    {
        return Context.User?.GetMemberId()?? throw new HubException("Can't Get User Id");
    }

}