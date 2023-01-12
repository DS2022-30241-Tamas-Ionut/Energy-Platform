using Microsoft.AspNetCore.SignalR;

namespace Assignment1.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task SendIsTyping(string user)
        {
            await Clients.All.SendAsync("ReceiveTyping", user, $"{user} is typing ...");
        }

        public async Task NotTyping()
        {
            await Clients.All.SendAsync("StoppedTyping");
        }

        public async Task Read(string user)
        {
            await Clients.All.SendAsync("MessageRead", user);
        }
    }
}
