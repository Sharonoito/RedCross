using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using NuGet.Protocol.Core.Types;
using RedCrossChat.Contracts;

using System.Collections.Generic;
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
            var visits= await _repository.Conversation.FindAll().ToListAsync();

            var maleVisits=await _repository.Conversation.FindByCondition(x => x.Client.Gender == "Male" ).ToListAsync();

            List<ReportVm> reportVm = new List<ReportVm>()
            {
                new ReportVm("Total Visits","Highest on sunday",visits.Count,"10","bx bxs-truck","primary"),
                new ReportVm("Total Male Visits","Increase From Last week",maleVisits.Count,"31","bx bxs-truck","warning"),
            };

            return View(reportVm);
        }

        [HttpGet]
        public IActionResult Dashboard()
        {
            return View();
        }

    }
}
