using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ModWebsite.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        [Authorize("")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
