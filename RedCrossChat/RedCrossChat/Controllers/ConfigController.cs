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
        #region Feeling
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
        public async Task<IActionResult> EditFeeling(Guid clientId)
        
        {
            try
            {
                var feelingEntity = _repository.Feeling.FindByCondition(x => x.Id == clientId).FirstOrDefault();
                if (feelingEntity == null)
                {
                    return NotFound(); 
                }

                var feelingViewModel = new FeelingVm
                {
                    Id = feelingEntity.Id,
                    Name = feelingEntity.Name,
                    //Description = feelingEntity.Description,
                    Kiswahili=feelingEntity.Kiswahili,
                    Synonyms = feelingEntity.Synonymns
                };

                ViewBag.Title = "Edit Feeling";
                return View("_Feeling", feelingViewModel);
            }
            catch (Exception ex)
            {
                return Error("Something broke" + ex.Message);
            }
        }

        //[HttpPost]
        public async Task<IActionResult> DeleteFeeling(Guid id)
        {
            try
            {
                var feelingEntity = await _repository.Feeling.FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
                if (feelingEntity == null)
                {
                    return NotFound();
                }

                _repository.Feeling.Delete(feelingEntity);
                var result = await _repository.SaveChangesAsync();

                if (!result)
                {
                    return Error("Error deleting Feeling");
                }

                return Success("Feeling deleted successfully");
            }
            catch (Exception ex)
            {
                return Error("Something broke" + ex.Message);
            }
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
                        Kiswahili=feeling.Kiswahili,
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

#endregion

        #region Professional
        public IActionResult Profession()
        {
            return View();
        }

        public IActionResult CreateProfession()
        {
            ViewBag.Title = "Create Profession";

            return View("_Profession");
        }

        [HttpPost]
        public async Task<IActionResult> GetProfession(IDataTablesRequest dtRequest)
        {

            try
            {
                var data = await _repository.Profession.GetAllAsync();

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

        [HttpPost]
        public async Task<IActionResult> SaveProfession(ProfessionVm profession)
        {
            if (!ModelState.IsValid)
                return Error("Validation error!, please check your data.");


            try
            {
                if (profession.Id == Guid.Empty)
                {
                    var professionEntity = new Profession
                    {
                        Name = profession.Name,
                        Kiswahili = profession.Kiswahili,
                        Synonyms = profession.Synonyms
                    };

                    _repository.Profession.Create(professionEntity);


                    var result = await _repository.SaveChangesAsync();

                    if (!result)
                        return Error("Error Creating Proffession!");
                }
                else
                {
                    var professionDB = await _repository.Profession.FindByCondition(x => x.Id == profession.Id).FirstOrDefaultAsync();

                    if (professionDB == null)
                    {
                        return Error("Profession not found");
                    }


                    professionDB.Name = profession.Name;
                    professionDB.Kiswahili = profession.Kiswahili;
                    professionDB.Synonyms = profession.Synonyms;

                    _repository.Profession.Update(professionDB);

                    var result = await _repository.SaveChangesAsync();

                    if (!result)
                        // return Success(null, null);
                        return Error("Error updating profession");
                }
            }
            catch (Exception ex)
            {
                return Error("Something broke" + ex.Message);
            }
            return Success("Profession Saved successfully");
        }


        public async Task<IActionResult> EditProfession(Guid clientId)

        {
            try
            {
                var professionEntity = _repository.Profession.FindByCondition(x => x.Id == clientId).FirstOrDefault();
                if (professionEntity  == null)
                {
                    return NotFound();
                }

                var professionViewModel = new ProfessionVm
                {
                    Id = professionEntity.Id,
                    Name = professionEntity.Name,
                    Kiswahili= professionEntity.Kiswahili,
                    Synonyms = professionEntity.Synonyms
                };

                ViewBag.Title = "Edit Profession";
                return View("_Profession", professionViewModel);
            }
            catch (Exception ex)
            {
                return Error("Something broke" + ex.Message);
            }
        }

        public async Task<IActionResult> DeleteProfession(Guid id)
        {
            try
            {
                var professionEntity = await _repository.Profession.FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
                if (professionEntity == null)
                {
                    return NotFound();
                }

                _repository.Profession.Delete(professionEntity);
                var result = await _repository.SaveChangesAsync();

                if (!result)
                {
                    return Error("Error deleting Profession");
                }

                return Success("Profession deleted successfully");
            }
            catch (Exception ex)
            {
                return Error("Something broke" + ex.Message);
            }
        }

        #endregion

        #region AgeBand
        public IActionResult AgeBand()
        {
            return View();
        }

        public IActionResult CreateAgeBand()
        {
            ViewBag.Title = "Create AgeBand";

            return View("_AgeBand");
        }


        [HttpPost]
        public async Task<IActionResult> GetAgeBand(IDataTablesRequest dtRequest)
        {

            try
            {
                var data = await _repository.AgeBand.GetAllAsync();

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

        public async Task<IActionResult> EditAgeBand(Guid clientId)

        {
            try
            {
                var agebandEntity = _repository.AgeBand.FindByCondition(x => x.Id == clientId).FirstOrDefault();
                if (agebandEntity == null)
                {
                    return NotFound();
                }

                var ageBandViewModel = new AgeBandVm
                {
                    Id = agebandEntity.Id,
                    Name = agebandEntity.Name,
                    Kiswahili=agebandEntity.Kiswahili,
                    Synonyms = agebandEntity.Synonyms
                };

                ViewBag.Title = "Edit AgeBand";
                return View("_AgeBand", ageBandViewModel);
            }
            catch (Exception ex)
            {
                return Error("Something broke" + ex.Message);
            }
        }



        [HttpPost]
        public async Task<IActionResult> SaveAgeBand(AgeBandVm ageBand)

        {
            if (!ModelState.IsValid)
                return Error("Validation error!, please check your data.");


            try
            {
                if (ageBand.Id == Guid.Empty)
                {
                    var ageBandEntity = new AgeBand
                    {
                        Name = ageBand.Name,
                        Kiswahili = ageBand.Kiswahili,
                        Synonyms = ageBand.Synonyms
                    };

                    _repository.AgeBand.Create(ageBandEntity);


                    var result = await _repository.SaveChangesAsync();

                    if (!result)
                        return Error("Error Creating AgeBand!");
                }
                else
                {
                    var ageBandDB = await _repository.AgeBand.FindByCondition(x => x.Id == ageBand.Id).FirstOrDefaultAsync();

                    if (ageBandDB == null)
                    {
                        return Error("AgeBand not found");
                    }


                    ageBand.Name = ageBand.Name;
                    ageBand.Kiswahili = ageBand.Kiswahili;
                    ageBand.Synonyms = ageBand.Synonyms;

                    _repository.AgeBand.Update(ageBandDB);

                    var result = await _repository.SaveChangesAsync();

                    if (!result)
                        return Success(null, null);
                    return Error("Error updating ageBand");
                }
            }
            catch (Exception ex)
            {
                return Error("Something broke" + ex.Message);
            }
            return Success("AgeBand Saved successfully");
        }

        public async Task<IActionResult> DeleteAgeBand(Guid id)
        {
            try
            {
                var ageBandEntity = await _repository.AgeBand.FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
                if (ageBandEntity == null)
                {
                    return NotFound();
                }

                _repository.AgeBand.Delete(ageBandEntity);
                var result = await _repository.SaveChangesAsync();

                if (!result)
                {
                    return Error("Error deleting AgeBand");
                }

                return Success("AgeBand deleted successfully");
            }
            catch (Exception ex)
            {
                return Error("Something broke" + ex.Message);
            }
        }

        #endregion

        #region MaritalState
        public IActionResult MaritalState()
        {
            return View();
        }

        public IActionResult CreateMaritalState()
        {
            ViewBag.Title = "Create MaritalState";

            return View("_MaritalState");
        }


        [HttpPost]
        public async Task<IActionResult> GetMaritalState(IDataTablesRequest dtRequest)
        {

            try
            {
                var data = await _repository.MaritalState.GetAllAsync();

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

        public async Task<IActionResult> EditMaritalState(Guid clientId)

        {
            try
            {
                var maritalEntity = _repository.MaritalState.FindByCondition(x => x.Id == clientId).FirstOrDefault();
                if (maritalEntity == null)
                {
                    return NotFound();
                }

                var maritalStateViewModel = new MaritalStateVm
                {
                    Id = maritalEntity.Id,
                    Name = maritalEntity.Name,
                    Kiswahili= maritalEntity.Kiswahili,
                    Synonyms = maritalEntity.Synonyms
                };

                ViewBag.Title = "Edit MaritalState";
                return View("_MaritalState", maritalStateViewModel);
            }
            catch (Exception ex)
            {
                return Error("Something broke" + ex.Message);
            }
        }



        [HttpPost]
        public async Task<IActionResult> SaveMaritalState(MaritalStateVm maritalState)

        {
            if (!ModelState.IsValid)
                return Error("Validation error!, please check your data.");


            try
            {
                if (maritalState.Id == Guid.Empty)
                {
                    var maritalStateEntity = new MaritalState
                    {
                        Name = maritalState.Name,
                        Kiswahili=maritalState.Kiswahili,
                        Synonyms = maritalState.Synonyms
                    };

                    _repository.MaritalState.Create(maritalStateEntity);


                    var result = await _repository.SaveChangesAsync();

                    if (!result)
                        return Error("Error Creating Marital State!");
                }

                else
                {
                    var maritalStateDB = await _repository.MaritalState.FindByCondition(x => x.Id == maritalState.Id).FirstOrDefaultAsync();

                    if (maritalStateDB == null)
                    {
                        return Error("Marital Status not found");
                    }


                    maritalStateDB.Name = maritalState.Name;
                    maritalStateDB.Kiswahili = maritalState.Kiswahili;
                    maritalStateDB.Synonyms = maritalState.Synonyms;

                    _repository.MaritalState.Update(maritalStateDB);

                    var result = await _repository.SaveChangesAsync();

                    if (!result)
                        // return Success(null, null);
                        return Error("Error updating Marital State");
                }
            }
            catch (Exception ex)
            {
                return Error("Something broke" + ex.Message);
            }
            return Success("Marital State Saved successfully");
        }


        #endregion

        #region Gender
        public IActionResult Gender()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetGender(IDataTablesRequest dtRequest)
        {

            try
            {
                var data = await _repository.Gender.GetAllAsync();

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

        public IActionResult CreateGender()
        {
            ViewBag.Title = "Create Gender";

            return View("_Gender");
        }

        public async Task<IActionResult> EditGender(Guid clientId)

        {
            try
            {
                var genderEntity = _repository.Gender.FindByCondition(x => x.Id == clientId).FirstOrDefault();
                if (genderEntity == null)
                {
                    return NotFound();
                }

                var genderViewModel = new GenderVm
                {
                    Id = genderEntity.Id,
                    Name = genderEntity.Name,
                    Kiswahili=genderEntity.Kiswahili,
                
                };

                ViewBag.Title = "Edit Gender";
                return View("_Gender", genderViewModel);
            }
            catch (Exception ex)
            {
                return Error("Something broke" + ex.Message);
            }
        }


        public async Task<IActionResult> DeleteGender(Guid id)
        {
            try
            {
                var genderEntity = await _repository.Gender.FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
                if (genderEntity == null)
                {
                    return NotFound();
                }

                _repository.Gender.Delete(genderEntity);
                var result = await _repository.SaveChangesAsync();

                if (!result)
                {
                    return Error("Error deleting Gender");
                }

                return Success("Gender deleted successfully");
            }
            catch (Exception ex)
            {
                return Error("Something broke" + ex.Message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> SaveGender(GenderVm gender)
        {
            if (!ModelState.IsValid)
                return Error("Validation error!, please check your data.");


            try
            {
                if (gender.Id == Guid.Empty)
                {
                    var genderEntity = new Entities.Gender
                    {
                        Name = gender.Name,
                        Kiswahili=gender.Kiswahili,
                    };

                    _repository.Gender.Create(genderEntity);


                    var result = await _repository.SaveChangesAsync();

                    if (!result)
                        return Error("Error Creating Gender!");
                }
                else
                {

                    var genderDB = await _repository.Gender.FindByCondition(x => x.Id == gender.Id).FirstOrDefaultAsync();

                    if (genderDB == null)
                    {
                        return Error("Gender not found");
                    }


                    genderDB.Name = gender.Name;
                    genderDB.Kiswahili = gender.Kiswahili;

                    _repository.Gender.Update(genderDB);

                    var result = await _repository.SaveChangesAsync();

                    if (!result)
                        // return Success(null, null);
                        return Error("Error updating Gender");
                }
            }
            catch (Exception ex)
            {
                return Error("Something broke" + ex.Message);
            }
            return Success("Gender Saved successfully");
        }



        #endregion


    }
}


