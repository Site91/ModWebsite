using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mod.DataAccess;
using Mod.DataAccess.Repository;
using Mod.DataAccess.Repository.IRepository;
using Mod.Models.ViewModels.Sockets;
using Mod.Util;
using ModWebsite.Areas.Home.Controllers;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Cms;
using SixLabors.ImageSharp.Memory;
using System;
using System.Net;
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
        private readonly WebSocketUtil _websocket;

        public WebSocketController(ILogger<WebSocketController> logger, IServiceScopeFactory service, IUnitOfWork unitOfWork, WebSocketUtil wsU)
        {
            _logger = logger;
            _service = service;
            _unitOfWork = unitOfWork;
            _websocket = wsU;
        }

        [HttpPost]
        public async Task<IActionResult> SocketRequest(string siteKey)
        {
            Stream req = Request.Body;
            req.Seek(0, SeekOrigin.Begin);
            string jsonS = new StreamReader(req).ReadToEnd();
            JsonObject json;
            try
            {
                json = JsonObject.Parse(jsonS).AsObject();
            }
            catch 
            {
                return BadRequest();
            }
            if (json == null)
                return BadRequest();
            var e = await _websocket.Send(siteKey, json);
            return Ok(json);
        }

        String theCode = "abcdfff";

        [Area("Server")]
        [Route("/ws")]
        public async Task Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var site = _unitOfWork.AuthorizedSites.GetFirstOrDefault(u => u.AccessCode == HttpContext.Request.Headers.Authorization.ToString());
                if (site != null) //site authorized
                {
                    using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                    var heldSocket = new SocketHolder(webSocket, HttpContext.Request.Headers.Authorization.ToString(), site.AccessURL);
                    await _websocket.serverWebsocket(heldSocket);
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
