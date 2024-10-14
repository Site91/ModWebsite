using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Mod.DataAccess.Repository.IRepository;
using Mod.Models;
using Mod.Models.ViewModels.Sites;
using Mod.Util;
using ModWebsite.Areas.Server.Controllers;
using NuGet.Protocol;
using System.Text.Json.Nodes;

namespace ModWebsite.Areas.Site.Controllers
{
    [Area("Site")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly WebSocketUtil _websocket;
        private readonly JSONSaver _jsonSaver;
        private readonly IMemoryCache _memoryCache;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, WebSocketUtil websocket, JSONSaver jsonSaver, IMemoryCache memoryCache, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _websocket = websocket;
            _jsonSaver = jsonSaver;
            _memoryCache = memoryCache;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public IActionResult Index(string site)
        {
            if (SiteExists(site, out AuthorizedSites siteData, _unitOfWork))
            {
                var obj = new SiteIndexVM()
                {
                    SiteData = siteData,
                    SiteObj = _jsonSaver.Load(siteData.Directory)
                };
                if (obj.SiteObj != null) //file exists
                {
                    return View(obj);
                }
                return StatusCode(500); //file is missing and should be fixed by admin
            }
            return NotFound(); //Site does not exist
        }



        #region WebAPIRequests to Websockets
        //API requests made by clients to the Website, requesting to get something through the websocket.
        //[Authorize] //TODO: Authorize later
        [HttpGet]
        public async Task<IActionResult> GetActiveUsers(string site)
        {
            if(SiteExists(site, out AuthorizedSites siteData, _unitOfWork))
            {
                JsonObject json = new JsonObject();
                json.Add("reqtype", "playerlist");
                var returned = await _websocket.Send(site, json);
                if (returned != null && (bool)returned.FirstOrDefault(u=>u.Key == "socketWorked").Value == true) //success
                {
                    return Json(new { success = true, names = returned.FirstOrDefault(u => u.Key == "playerNames").Value, uuids = returned.FirstOrDefault(u => u.Key == "playerUUIDs").Value });
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
                if (SiteExists(site, out AuthorizedSites siteData, _unitOfWork))
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
        private bool SiteExists(string site, out AuthorizedSites siteData, IUnitOfWork unitOfWork)
        {
            siteData = unitOfWork.AuthorizedSites.GetFirstOrDefault(u => u.AccessURL == site);
            return siteData != null;
        }
        private JsonObject GetData(string site) //get from memory or from file
        {
            if (SiteExists(site, out AuthorizedSites siteData, _unitOfWork))
            {
                //Attempt to get from memory
                var inCache = _memoryCache.TryGetValue($"SiteData{siteData.Directory}", out JsonObject obj);
                if (inCache)
                {
                    return obj;
                }
                else
                {
                    obj = _jsonSaver.Load(siteData.Directory);
                    if (obj != null) //got the object
                    {
                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromHours(3))
                            .RegisterPostEvictionCallback(SiteEvictionCallback, site);

                        _memoryCache.Set($"SiteData{siteData.Directory}", obj, cacheEntryOptions);
                    }
                }
            }
            return null;
        }
        private void CheckFileVer(AuthorizedSites site, JsonObject obj, JSONSaver jsonSaver)
        {
            var diskObj = jsonSaver.Load(site.Directory);
            if (diskObj != null)
            {
                if ((int)diskObj.First(u => u.Key == "updateID").Value.AsValue() != (int)obj.First(u => u.Key == "updateID").Value.AsValue()) //difference in version
                {
                    jsonSaver.Save(site.Directory, obj);
                }
            }
        }

        /* Site JSON Object
         * (MAIN STUFF [base 
         * revision = int. Checks if an update is needed to the JSON.
         * updateID = current version of the file. If different from one in memory, overwrite the disk file.
         * 
         */

        private void SiteEvictionCallback(object cacheKey, object cacheValue, EvictionReason evictionReason, object state)
        {
            var site = (string)state;
            var obj = (JsonObject)cacheValue;
            var key = (string)cacheKey;
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var jsonSaver = scope.ServiceProvider.GetRequiredService<JSONSaver>();
                var memoryCache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                if (SiteExists(site, out AuthorizedSites siteData, _unitOfWork))
                {
                    CheckFileVer(siteData, obj, jsonSaver);
                }
            }
        }
            #endregion
    }
}
