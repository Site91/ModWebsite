using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mod.DataAccess;
using Mod.DataAccess.Repository;
using Mod.DataAccess.Repository.IRepository;
using Mod.Models.ViewModels.Sockets;
using ModWebsite.Areas.Home.Controllers;
using Org.BouncyCastle.Asn1.Cms;
using SixLabors.ImageSharp.Memory;
using System;
using System.Net.WebSockets;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.Json.Nodes;

namespace ModWebsite.Areas.Server.Controllers
{
    public class WebSocketController : ControllerBase
    {
        //libs
        private readonly IServiceScopeFactory _service;
        private readonly ILogger<WebSocketController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        private int nextId = 0;

        public WebSocketController(ILogger<WebSocketController> logger, IServiceScopeFactory service, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _service = service;
            _unitOfWork = unitOfWork;
        }

        //internal vars
        public List<SocketHolder> connections = new List<SocketHolder>();
        public Queue<WebSocket> closed = new Queue<WebSocket>();

        //public Queue<>

        private async Task Echo(WebSocket webSocket)
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

        private async Task serverWebsocket(SocketHolder webSocket, IUnitOfWork unitOfWork)
        {
            var buffer = new byte[1024 * 4];
            var receiveResult = await webSocket.socket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!receiveResult.CloseStatus.HasValue)
            {
                bool submit = false;
                var returned = new JsonObject();
                if (receiveResult.Count > 0)
                {
                    var e = JsonObject.Parse(Encoding.UTF8.GetString(buffer).Substring(0, receiveResult.Count)).AsObject();
                    Console.WriteLine(e.ToString());
                    if (e.TryGetPropertyValue("reqtype", out JsonNode reqType) && ((string?)reqType.AsValue()) != null)
                    {
                        returned.Add("reqtype", (string?)reqType.AsValue());
                        if (e.TryGetPropertyValue("clientid", out JsonNode clientId) && ((int?)clientId.AsValue()) != null)
                        {
                            returned.Add("clientid", (int?)clientId.AsValue()); //pass client ID back, hi from science :>
                        }
                        switch ((string?)reqType.AsValue())
                        {
                            case "getrealtime":
                                if (e.TryGetPropertyValue("uuid", out JsonNode uuid))
                                {
                                    returned.Add("uuid", (string?)uuid.AsValue());
                                }
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
                    returned.Add("status", false);
                    returned.Add("message", "no response");
                    submit = true;
                }
                if (submit == true)
                {
                    string doneJson = returned.ToString();
                    buffer = Encoding.UTF8.GetBytes(doneJson);
                    await webSocket.socket.SendAsync(
                        new ArraySegment<byte>(buffer, 0, doneJson.Length),
                        receiveResult.MessageType,
                        receiveResult.EndOfMessage,
                        CancellationToken.None);
                }
                receiveResult = await webSocket.socket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.socket.CloseAsync(
                receiveResult.CloseStatus.Value,
                receiveResult.CloseStatusDescription,
                CancellationToken.None);
            connections.Remove(webSocket);
        }

        public async Task<WebSocketReceiveResult> receiveAsync(WebSocket socket, ArraySegment<byte> buffer, CancellationToken token, bool overrided)
        {
            WebSocketReceiveResult e;
            e = await socket.ReceiveAsync(buffer, token);
            return e;
        }

        public async Task Send(string site)
        {
            var socket = connections.FirstOrDefault(u=>u.key == site);
            if(socket != null)
            {
                //Make request holder
                var request = new heldRequest();
                request.requestID = nextId;
                nextId++;
                request.siteKey = socket.key;

                var buffer = new byte[1024 * 4];
                string doneJson = returned.ToString();
                buffer = Encoding.UTF8.GetBytes(doneJson);
                await socket.socket.SendAsync(
                    new ArraySegment<byte>(buffer, 0, doneJson.Length),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);

                while (!request.finished)
                {
                    await Task.Delay(25);
                }
            }
        }

        public async Task Broadcast(string msg)
        {
            var bytes = Encoding.UTF8.GetBytes(msg);
            foreach (var socket in connections)
            {
                if (socket.socket.State == WebSocketState.Open)
                {
                    var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
                    await socket.socket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
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
                if (HttpContext.Request.Headers.Authorization == theCode || false)
                {
                    using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                    var heldSocket = new SocketHolder(webSocket, HttpContext.Request.Headers.Authorization.ToString());
                    connections.Add(heldSocket);
                    await serverWebsocket(heldSocket, _unitOfWork);
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
    public class heldRequest()
    {
        public bool finished = false;
        public int requestID;
        public string siteKey;
    }
}
