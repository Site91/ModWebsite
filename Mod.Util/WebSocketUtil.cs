using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mod.DataAccess.DbInitializer;
using Mod.DataAccess.Repository.IRepository;
using Mod.Models.ViewModels.Sockets;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Mod.Util
{
    public class WebSocketUtil
    {
        private readonly IServiceProvider _serviceProvider;

        public static List<SocketHolder> connections = new List<SocketHolder>();
        public static List<heldRequest> requests = new List<heldRequest>();
        //public Queue<WebSocket> closed = new Queue<WebSocket>();

        public WebSocketUtil(IServiceProvider socketFactory)
        {
            _serviceProvider = socketFactory;
        }
        //public async Task SendMessage(string serverLink, JsonObject json)
        //{

        //}
        private int nextId = 0;
        public async Task serverWebsocket(SocketHolder webSocket)
        {
            connections.Add(webSocket);
            using (var scope = _serviceProvider.CreateScope())
            {
                var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var _logger = scope.ServiceProvider.GetRequiredService<ILogger<WebSocketUtil>>();
                _logger.LogInformation("Starting up websocket connection with key " + webSocket.key);
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
                        if (e.TryGetPropertyValue("serverid", out JsonNode serverId))
                        {
                            //Search connections
                            var connection = requests.FirstOrDefault(u => u.siteKey == webSocket.key && u.requestID == ((int?)serverId.AsValue()) && !u.finished);
                            if (connection != null)
                            {
                                connection.json = e;
                                connection.worked = true;
                                connection.finished = true;
                            }
                            else
                            {
                                //drop the request.
                            }
                        }
                        else if (e.TryGetPropertyValue("reqtype", out JsonNode reqType) && ((string?)reqType.AsValue()) != null)
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
                _logger.LogWarning("Closing connection with " + webSocket.key);
                await webSocket.socket.CloseAsync(
                    receiveResult.CloseStatus.Value,
                    receiveResult.CloseStatusDescription,
                    CancellationToken.None);
                requests.ForEach(request => {
                    if (request.siteKey == webSocket.key)
                    {
                        request.finished = true;
                    }
                });
                connections.Remove(webSocket);
            }
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
                if (socket.socket.State == WebSocketState.Open)
                {
                    var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
                    await socket.socket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }

        public async Task<JsonObject> Send(string site, JsonObject json, bool getBack = true)
        {
            var socket = connections.FirstOrDefault(u => u.url == site);
            if (socket != null)
            {
                //Make request holder
                var request = new heldRequest();
                request.requestID = nextId;
                nextId++;
                request.siteKey = socket.key;
                //Put data into json
                json.Add("serverid", request.requestID);
                //Put request into list
                if(getBack )
                    requests.Add(request);

                var buffer = new byte[1024 * 4];
                string doneJson = json.ToString();
                buffer = Encoding.UTF8.GetBytes(doneJson);
                await socket.socket.SendAsync(
                    new ArraySegment<byte>(buffer, 0, doneJson.Length),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);
                if (getBack)
                {
                    int timer = 15 * 1000; //in seconds
                    while (!request.finished || timer <= 0)
                    {
                        await Task.Delay(25);
                        timer -= 25;
                    }
                    requests.Remove(request);
                    if (request.worked)
                    {
                        request.json.Add("socketWorked", true);
                        return request.json;
                    }
                }
                else
                {
                    return json;
                }
            }
            json.Add("socketWorked", false); //return original json
            return json;
        }
        public async Task<JsonObject> ProcessBody(Stream req)
        {
            req.Seek(0, SeekOrigin.Begin);
            string jsonS = new StreamReader(req).ReadToEnd();
            JsonObject json;
            try
            {
                json = JsonObject.Parse(jsonS).AsObject();
            }
            catch
            {
                return null;
            }
            return json;
        }
    }
}
public class heldRequest()
{
    public bool finished = false;
    public bool worked = false;
    public int requestID;
    public string siteKey;
    public JsonObject json;
}
