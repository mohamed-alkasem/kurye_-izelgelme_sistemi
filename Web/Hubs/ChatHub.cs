using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace Web.Hubs
{
    public class ChatHub : Hub
    {
        private static ConcurrentDictionary<string, string> names = new ConcurrentDictionary<string, string>();
        private static ConcurrentDictionary<string, string> connections = new ConcurrentDictionary<string, string>();

        public override Task OnConnectedAsync()
        {
            var username = Context.GetHttpContext()!.Request.Query["username"];
            var fullname = Context.GetHttpContext()!.Request.Query["fullname"];
            if (!string.IsNullOrWhiteSpace(username))
            {
                connections[username!] = Context.ConnectionId;
                if (!string.IsNullOrWhiteSpace(fullname))
                    names[username!] = fullname!;
            }
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var item = connections.FirstOrDefault(x => x.Value == Context.ConnectionId);
            if (!string.IsNullOrWhiteSpace(item.Key))
            {
                connections.TryRemove(item.Key, out _);
                names.TryRemove(item.Key, out _);
            }
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string fromUser, string toUser, string message)
        {
            names.TryGetValue(fromUser, out var fromName);
            var displayName = string.IsNullOrWhiteSpace(fromName) ? fromUser : fromName;
            if (connections.TryGetValue(toUser, out var toConnection))
            {
                await Clients.Client(toConnection).SendAsync("ReceiveMessage", displayName, message);
            }
            if (connections.TryGetValue(fromUser, out var fromConnection))
            {
                await Clients.Client(fromConnection).SendAsync("ReceiveMessage", displayName, message);
            }
        }
    }
}
