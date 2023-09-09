using Microsoft.AspNetCore.Mvc;

namespace RedCrossChat
{
    public class HomeController : Controller
    {


        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Dashboard()
        {
            return View();
        }

    }
}
