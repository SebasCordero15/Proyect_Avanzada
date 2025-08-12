using Microsoft.AspNetCore.Mvc;

namespace PAW.Mvc.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
    }
}
