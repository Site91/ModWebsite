using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mod.DataAccess.Repository.IRepository;
using Mod.Models;
using Mod.Util;
using ModWebsite.Areas.Server.Controllers;
using System.Text.Json.Nodes;

namespace ModWebsite.Areas.Site.Controllers
{
    [Area("Site")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly WebSocketUtil _websocket;
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, WebSocketUtil websocket)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _websocket = websocket;
        }

        public IActionResult Index()
        {
            return View();
        }



        #region WebAPIRequests to Websockets
        //API requests made by clients to the Website, requesting to get something through the websocket.
        //[Authorize] //TODO: Authorize later
        [HttpGet]
        public async Task<IActionResult> GetActiveUsers(string site)
        {
            if(SiteExists(site, out AuthorizedSites siteData))
            {
                JsonObject json = new JsonObject();
                json.Add("reqtype", "playerlist");
                var returned = await _websocket.Send(site, json);
                if (returned != null && (bool)returned.FirstOrDefault(u=>u.Key == "socketWorked").Value == true) //success
                {
                    return Json(new {success = true, names = returned.FirstOrDefault(u=>u.Key == "players").Value.AsObject().Select(u => u.Value.ToString()) });
                }
                else
                {
                    //return StatusCode(500); //internal server error
                    return Json(new { success = false, error = "Request failed to make it through the websocket." });
                }
            }
            return Json(new {success = false, error = "Request failed to make it through the websocket."}); //They are the same to prevent users from finding the correct site key.
        }
        //[Authorize] //TODO: Authorize later
        [HttpPost]
        public async Task<IActionResult> KillUser(string site) {
            JsonObject json = await _websocket.ProcessBody(Request.Body);
            if (json != null)
            {
                if(!json.ContainsKey("username") && !json.ContainsKey("uuid"))
                {
                    return Json(new { success = false, error = "Missing username or uuid" });
                }
                if (SiteExists(site, out AuthorizedSites siteData))
                {
                    json.Add("reqtype", "playerkill");
                    await _websocket.Send(site, json, false);
                    return Json(new { success = true});
                }
                return Json(new { success = false, error = "Request failed to make it through the websocket." });
            }
            return Json(new { success = false, error = "Invalid JSON Formatting" });
        }

        #endregion

        #region Helpers
        //functions used by all
        private bool SiteExists(string site, out AuthorizedSites siteData)
        {
            siteData = _unitOfWork.AuthorizedSites.GetFirstOrDefault(u => u.AccessURL == site);
            return siteData != null;
        }
        #endregion
    }
}
