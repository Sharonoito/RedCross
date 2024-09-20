using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RedCrossChat.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
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

            var ageBands = await _repository.AgeBand.GetAll();

            var genders=await _repository.Gender.FindAll().ToListAsync();

            var handOverRequest = await _repository.HandOverRequest.FindByCondition(x => x.AppUserId== Guid.Parse(User.FindFirst("UserId").Value)).ToListAsync();


            return View(new ReportVm { feelings=feelings,countys=county,Genders=genders,HandOverRequests=handOverRequest,Agebands=ageBands.ToList() });
        }

        [HttpGet]
        public IActionResult Dashboard()
        {
            return View();
        }

        public async Task<IActionResult> GenderGraph()
        {
            var genders = await _repository.Gender.FindAll().ToListAsync();
            return View();
        }

        [HttpGet, HttpPost]
        public JsonResult GetReportData(int Type=0)
        {
            string ConnectionString=System.Configuration.ConfigurationManager.ConnectionStrings["LocalConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                string Query = "";



            }

            return base.Json(new { });
        }





    }
}
