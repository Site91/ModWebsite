using Microsoft.AspNetCore.Mvc;
using Mod.DataAccess.Repository.IRepository;
using Mod.Models;
using ModWebsite.Areas.Server.Controllers;
using System.Text.Json.Nodes;

namespace ModWebsite.Areas.Site.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }



        #region WebAPIRequests to Websockets
        //API requests made by clients to the Website, requesting to get something through the websocket.
        [HttpGet]
        public async Task<IActionResult> GetActiveUsers(string site)
        {
            if(SiteExists(site, out AuthorizedSites siteData))
            {
                JsonObject json = new JsonObject();
                json.Add("reqtype", "playerlist");
                WebSocketController
            }
            return BadRequest();
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
