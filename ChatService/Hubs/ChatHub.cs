using ChatService.DataService;
using ChatService.Models;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.Hubs
{
    public class ChatHub : Hub
    {
        private readonly SharedDb _sharedDb;
        public ChatHub(SharedDb sharedDb)
        {
            _sharedDb = sharedDb;
        }
        public async Task JoinChat(UserConnection conn)
        {
            await Clients.All.SendAsync("ReceiveMessage", "admin", $"{conn.Username} has joined");
        }

        public async Task JoinSpecificChatRoom(UserConnection conn)
        {
            _sharedDb.Connections[Context.ConnectionId] = conn;

            await Groups.AddToGroupAsync(Context.ConnectionId, conn.ChatRoom);
            await Clients.Group(conn.ChatRoom).SendAsync("JoinSpecificChatRoom", "admin", $"{conn.Username} has joined {conn.ChatRoom}");
        }

        public async Task SendMessage(string message)
        {
            if (_sharedDb.Connections.TryGetValue(Context.ConnectionId, out UserConnection conn))
            {
                await Clients.Group(conn.ChatRoom).SendAsync("ReceiveSpecificMessage", conn.Username, message);
            }
        }
    }
}
