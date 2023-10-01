using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedCrossChat.Contracts;

using System.Threading.Tasks;

namespace RedCrossChat
{
    public class HomeController  : BaseController
    {
        private readonly IRepositoryWrapper _repository;

        public HomeController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
         
            var feelings=await _repository.Feeling.FindAll().ToListAsync();

            var county=await _repository.County.FindAll().ToListAsync();

            return View(new ReportVm { feelings=feelings,countys=county });
        }

        [HttpGet]
        public IActionResult Dashboard()
        {
            return View();
        }

       

    }
}
