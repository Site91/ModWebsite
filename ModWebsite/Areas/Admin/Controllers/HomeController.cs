using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using Mod.DataAccess.Repository.IRepository;
using Mod.Models;
using Mod.Utility;

namespace ModWebsite.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [Authorize(SD.Role_Manager)]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult CreateSite()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateSite(AuthorizedSites site)
        {
            if (ModelState.IsValid)
            {
                //First: add the authorized sites
                _unitOfWork.AuthorizedSites.Add(site); //save db at the end
                //Second: add base roles
                _unitOfWork.SiteRoles.Add(new SiteRoles()
                {
                    SafeName = "Owner",
                    Id = Guid.NewGuid(),
                    canModify = false,
                    canRemove = false,
                    SiteId = site.Id,
                    Permissions = "all",
                    AddLevel = 10
                });
                _unitOfWork.SiteRoles.Add(new SiteRoles()
                {
                    SafeName = "Admin",
                    Id = Guid.NewGuid(),
                    canModify = true,
                    canRemove = false,
                    SiteId = site.Id,
                    Permissions="all;~role.*",
                    AddLevel = 9
                });
                return View();
            }
            return View(site);
        }

    }
}
