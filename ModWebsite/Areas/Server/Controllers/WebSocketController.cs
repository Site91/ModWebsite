using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;

namespace ModWebsite.Areas.Server.Controllers
{
    public class WebSocketController : ControllerBase
    {
        public List<WebSocket> connections = new List<WebSocket>();
        public Queue<WebSocket> closed = new Queue<WebSocket>();

        private static async Task Echo(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            var receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!receiveResult.CloseStatus.HasValue)
            {
                await webSocket.SendAsync(
                    new ArraySegment<byte>(buffer, 0, receiveResult.Count),
                    receiveResult.MessageType,
                    receiveResult.EndOfMessage,
                    CancellationToken.None);

                receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(
                receiveResult.CloseStatus.Value,
                receiveResult.CloseStatusDescription,
                CancellationToken.None);
        }

        private static async Task serverWebsocket(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            var receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!receiveResult.CloseStatus.HasValue)
            {
                await webSocket.SendAsync(
                    new ArraySegment<byte>(buffer, 0, receiveResult.Count),
                    receiveResult.MessageType,
                    receiveResult.EndOfMessage,
                    CancellationToken.None);

                receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(
                receiveResult.CloseStatus.Value,
                receiveResult.CloseStatusDescription,
                CancellationToken.None);
        }

        public async Task<WebSocketReceiveResult> receiveAsync(WebSocket socket, ArraySegment<byte> buffer, CancellationToken token, bool overrided)
        {
            WebSocketReceiveResult e;
            while (true)
            {
                e = await socket.ReceiveAsync(buffer, token);
                if (e.CloseStatus.HasValue)
                    return e;
                if (overrided || closed.Count == 0)
                {
                    return e;
                }
            }
        }

        public async Task Broadcast(string msg)
        {
            var bytes = Encoding.UTF8.GetBytes(msg);
            foreach (var socket in connections)
            {
                if(socket.State == WebSocketState.Open)
                {
                    var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
                    await socket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }

        String theCode = "abcdfff";
        
        [Route("/ws")]
        public async Task Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                if (HttpContext.Request.Headers.Authorization == theCode)
                {
                    using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                    connections.Add(webSocket);
                    await serverWebsocket(webSocket);
                }
                else
                {
                    HttpContext.Response.StatusCode = 401;
                    await HttpContext.Response.WriteAsync("Invalid Code");
                }
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
    }

}
