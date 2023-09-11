using Microsoft.AspNetCore.Mvc;

namespace RedCrossChat.Controllers
{
    public class ConversationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult List()
        {
            return View();
        }
    }
}
