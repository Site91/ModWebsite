using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Cms;
using System.Net.WebSockets;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.Json.Nodes;

namespace ModWebsite.Areas.Server.Controllers
{
    public class WebSocketController : ControllerBase
    {
        public List<WebSocket> connections = new List<WebSocket>();
        public Queue<WebSocket> closed = new Queue<WebSocket>();

        //public Queue<>

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
                bool submit = false;
                var returned = new JsonObject();
                if (receiveResult.Count > 0) 
                {
                    var e = JsonObject.Parse(Encoding.UTF8.GetString(buffer).Substring(0, receiveResult.Count)).AsObject();
                    Console.WriteLine(e.ToString());
                    if(e.TryGetPropertyValue("reqtype",out JsonNode reqType) && ((string?)reqType.AsValue()) != null)
                    {
                        if (e.TryGetPropertyValue("clientid", out JsonNode clientId))
                        {
                            returned.Add("clientid", clientId); //pass client ID back
                        }
                        switch ((string?)reqType.AsValue())
                        {
                            case "getrealtime":
                                returned.Add("status", true);
                                returned.Add("time", DateTime.Now.ToString());
                                submit = true;
                                break;
                            default:
                                returned.Add("status", false);
                                returned.Add("message", "invalid reqtype");
                                submit = true;
                                break;
                        }
                    }
                    else
                    {
                        returned.Add("status", false);
                        returned.Add("message", "invalid reqtype");
                        submit = true;
                    }
                }
                else
                {
                    returned.Add("status",false);
                    returned.Add("message","no response");
                    submit = true;
                }
                if (submit == true)
                {
                    string doneJson = returned.ToString();
                    buffer = Encoding.UTF8.GetBytes(doneJson);
                    await webSocket.SendAsync(
                        new ArraySegment<byte>(buffer, 0, doneJson.Length),
                        receiveResult.MessageType,
                        receiveResult.EndOfMessage,
                        CancellationToken.None);
                }
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
            e = await socket.ReceiveAsync(buffer, token);
            return e;
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
        
        [Area("Server")]
        [Route("/ws")]
        public async Task Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                if (HttpContext.Request.Headers.Authorization == theCode || true)
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
