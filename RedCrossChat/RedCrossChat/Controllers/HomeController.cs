using Microsoft.AspNetCore.Mvc;

namespace RedCrossChat
{
    public class HomeController : Controller
    {


        public IActionResult Index()
        {
            return View();
        }
    }
}
