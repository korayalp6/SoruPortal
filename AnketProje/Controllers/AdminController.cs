using Microsoft.AspNetCore.Mvc;

namespace AnketProje.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
