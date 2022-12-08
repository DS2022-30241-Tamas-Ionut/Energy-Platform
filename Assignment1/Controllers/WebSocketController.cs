using Assignment1.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;

namespace Assignment1.Controllers
{
    public class WebSocketController : ControllerBase
    {
        private readonly IReceiveMessageService _receiveMessageService;

        public WebSocketController(IReceiveMessageService receiveMessageService)
        {
            _receiveMessageService = receiveMessageService;
        }

        public async Task Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await SendMessage(webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        private async Task SendMessage(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];

            while (!webSocket.CloseStatus.HasValue)
            {
                var message = await _receiveMessageService.ConsumeMessage();

                if (!String.IsNullOrEmpty(message))
                {
                    var messageToSend = Encoding.ASCII.GetBytes(message);

                    Console.WriteLine();
                    Console.WriteLine(message);
                    Console.WriteLine();

                    await webSocket.SendAsync(
                    new ArraySegment<byte>(messageToSend, 0, message.Length),
                        WebSocketMessageType.Text,
                        WebSocketMessageFlags.EndOfMessage,
                        CancellationToken.None);
                }
            }

            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed gracefully", CancellationToken.None);
        }
    }
}

