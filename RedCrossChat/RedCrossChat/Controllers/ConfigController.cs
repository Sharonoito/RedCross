using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RedCrossChat.Contracts;
using RedCrossChat.Entities.Auth;
using RedCrossChat.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using NuGet.Protocol.Core.Types;
using Microsoft.EntityFrameworkCore;
using RedCrossChat.Objects;
using RedCrossChat.ViewModel;
using Microsoft.Bot.Schema;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using Microsoft.EntityFrameworkCore.Internal;
using Sentry;

namespace RedCrossChat.Controllers
{
    public class ConfigController : BaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;

        private readonly IRepositoryWrapper _repository;


        public ConfigController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager, IRepositoryWrapper repository)
        {
            _userManager=userManager;
            _signInManager=signInManager;
            _roleManager=roleManager;
            _repository=repository;
        }

        public IActionResult Index() 
        { 
            return View();
        }

        public IActionResult Feeling()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetFeelings(IDataTablesRequest dtRequest)
        {
        
            try
            {
                var data = await _repository.Feeling.GetAllAsync();

                var filteredRows = data
                    .AsQueryable()
                    .FilterBy(dtRequest.Search, dtRequest.Columns);

                var pagedRows = filteredRows
                    .SortBy(dtRequest.Columns)
                    .Skip(dtRequest.Start)
                    .Take(dtRequest.Length);


                var response = DataTablesResponse.Create(dtRequest, data.Count(),
                    filteredRows.Count(), pagedRows);

                return new DataTablesJsonResult(response);

            }
            catch (Exception ex)
            {
                return Error(ex.Message);
              
            }
        }

     
        public IActionResult CreateFeeling()
        {
            ViewBag.Title = "Create Feeling";

            return View("_Feeling");
        }

        [HttpPost]
        public async Task<IActionResult>SaveFeeling(FeelingVm feeling)
        {
            if (!ModelState.IsValid)
                return Error("Validation error!, please check your data.");

       
            try
            {
                if (feeling.Id == Guid.Empty) 
                {
                    var feelingEntity = new DBFeeling
                    {
                        Name = feeling.Name,
                        Description = feeling.Description,
                        Kiswahili="TODO",
                        Synonymns = feeling.Synonyms
                    };

                    _repository.Feeling.Create(feelingEntity);


                    var result = await _repository.SaveChangesAsync();

                    if (!result)
                        return Error("Error Creating Feeling!");
                }
                else 
                {
                    var feelingDB = await _repository.Feeling.FindByCondition(x => x.Id == feeling.Id).FirstOrDefaultAsync();

                    if (feelingDB == null)
                    {
                        return Error("Feeling not found");
                    }


                    feelingDB.Name = feeling.Name;
                    feelingDB.Description = feeling.Description;
                    feelingDB.Synonymns = feeling.Synonyms;

                    _repository.Feeling.Update(feelingDB);

                    var result = await _repository.SaveChangesAsync();

                    if (!result)
                        // return Success(null, null);
                        return Error("Error updating Feeling");
                }        
            }
            catch (Exception ex)
            {
                return Error("Something broke" + ex.Message);
            }
            return Success("Feeling Saved successfully");
        }


    }
}


