using Microsoft.AspNetCore.Mvc;

namespace InternetProg2.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
