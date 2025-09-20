using Microsoft.AspNetCore.SignalR;

namespace API.Hubs;

public sealed class ChatHub : Hub
{
    private readonly Dictionary<string, string> _userGroups = new();

    public async Task AddToGroup(string groupName, string userName)
    {
        // Store user's group association
        _userGroups[Context.ConnectionId] = groupName;

        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await Clients.Group(groupName).SendAsync("ReceiveMessage", userName, $"User {userName} has been joined to {groupName}");
    }

    public async Task RemoveFromGroup(string groupName, string userName)
    {
        if (_userGroups.ContainsKey(Context.ConnectionId))
        {
            _userGroups.Remove(Context.ConnectionId);
        }

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        await Clients.Group(groupName).SendAsync("ReceiveMessage", userName, $"User {userName} has been disconnected from {groupName}");
    }

    public async Task SendMessage(string groupName, string userName, string message)
    {
        await Clients.Group(groupName).SendAsync("ReceiveMessage", userName, message);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // Clean up when user disconnects
        if (_userGroups.TryGetValue(Context.ConnectionId, out var groupName))
        {
            await Clients.Group(groupName).SendAsync("ReceiveMessage", Context.ConnectionId, $"A user has disconnected from group {groupName}.");
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            _userGroups.Remove(Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }
}