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
using System.ComponentModel.DataAnnotations;

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
            ViewBag.PageTitle = "Feelings";
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
                var feelingEntity = await _repository.Feeling.FindByCondition(x => x.Id == clientId).FirstOrDefaultAsync();
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
            if (!ModelState.IsValid && ModelState.ErrorCount >1)
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
            ViewBag.PageTitle = "Professional Status";
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
            if (!ModelState.IsValid  && ModelState.ErrorCount >1)
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
                var professionEntity =await _repository.Profession.FindByCondition(x => x.Id == clientId).FirstOrDefaultAsync();
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
            ViewBag.PageTitle = "Age Band";
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
                var agebandEntity = await _repository.AgeBand.FindByCondition(x => x.Id == clientId).FirstOrDefaultAsync();
                if (agebandEntity == null)
                {
                    return NotFound();
                }

                var ageBandViewModel = new AgeBandVM
                {
                    Id = agebandEntity.Id,
                    Name = agebandEntity.Name,
                    Kiswahili=agebandEntity.Kiswahili,
                    //Highest = agebandEntity.Highest,
                    //Lowest = agebandEntity.Lowest,
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
        public async Task<IActionResult> SaveAgeBand(AgeBandVM ageBand)
        {
            if (!ModelState.IsValid && ModelState.ErrorCount > 1)
                return Error("Validation error!, please check your data.");

            try
            {
                if (ageBand.Id == Guid.Empty)
                {
                    // Creating a new AgeBand
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
                    // Retrieving the existing AgeBand from the database
                    var ageBandDB = await _repository.AgeBand.FindByCondition(x => x.Id == ageBand.Id).FirstOrDefaultAsync();

                    if (ageBandDB == null)
                    {
                        return Error("AgeBand not found");
                    }

                    // Updating properties of ageBandDB with values from ageBand
                    ageBandDB.Name = ageBand.Name;
                    ageBandDB.Kiswahili = ageBand.Kiswahili;
                    ageBandDB.Synonyms = ageBand.Synonyms;

                    // Mark the entity as modified and save changes to the database
                    _repository.AgeBand.Update(ageBandDB);
                    var result = await _repository.SaveChangesAsync();

                    if (!result)
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
            ViewBag.PageTitle = "Marital Status";

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
                var maritalEntity = await _repository.MaritalState.FindByCondition(x => x.Id == clientId).FirstOrDefaultAsync();
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
            if (!ModelState.IsValid && ModelState.ErrorCount > 1)
                return Error("Validation error!, please check your data.");


            try
            {
                if (maritalState.Id == Guid.Empty)
                {
                    var maritalStateEntity = new MaritalState
                    {
                        Name = maritalState.Name,
                        Kiswahili=maritalState.Kiswahili,
                        Synonyms = maritalState.Synonyms ?? ""
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

        public async Task<IActionResult> DeleteMaritalState(Guid id)
        {
            try
            {
                var maritalStateEntity = await _repository.MaritalState.FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
                if (maritalStateEntity == null)
                {
                    return NotFound();
                }

                _repository.MaritalState.Delete(maritalStateEntity);
                var result = await _repository.SaveChangesAsync();

                if (!result)
                {
                    return Error("Error deleting Marital state");
                }

                return Success("Marital state deleted successfully");
            }
            catch (Exception ex)
            {
                return Error("Something broke" + ex.Message);
            }
        }


        #endregion

        #region Gender
        public IActionResult Gender()
        {
            ViewBag.PageTitle = "Gender";
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
                var genderEntity = await _repository.Gender.FindByCondition(x => x.Id == clientId).FirstOrDefaultAsync();
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
            if (!ModelState.IsValid && ModelState.ErrorCount > 1)
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

        #region AppUserTeam
        public IActionResult AppUserTeam() 
        { 
            return View();
        }

        public IActionResult CreateAppUserTeam()
        {
            ViewBag.Title = "Create AppUserTeam";

            return View("_AppUserTeam");
        }


        [HttpPost]
        public async Task<IActionResult> GetAppUserTeam(IDataTablesRequest dtRequest)
        {

            try
            {
                var data = await _repository.AppUserTeam.GetAllAsync();

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
        public async Task<IActionResult> SaveAppUserTeam(AppUserTeamVm appUserTeam)
        {
            if (!ModelState.IsValid  && ModelState.ErrorCount >1)
                return Error("Validation error!, please check your data.");


            try
            {
                if (appUserTeam.Id == Guid.Empty)
                {
                   /* var appUserTeamEntity = new AppUserTeam
                    {
                        AppUser = appUserTeam.Employee,
                        //AppUserId = appUserTeam.UserID,
                        //TeamId = appUserTeam.TeamID,
                        Team = appUserTeam.Team
                    };

                    _repository.AppUserTeam.Create(appUserTeamEntity);


                    var result = await _repository.SaveChangesAsync();*/

                   /* if (!result)
                        return Error("Error Creating AppUserTeam!");*/
                }
                else
                {
                    var appUserTeamDB = await _repository.AppUserTeam.FindByCondition(x => x.Id == appUserTeam.Id).FirstOrDefaultAsync();

                    if (appUserTeamDB == null)
                    {
                        return Error("AppUserTeam not found");
                    }


                    appUserTeam.Employee = appUserTeam.Employee;
                    appUserTeam.Team = appUserTeam.Team;

                    _repository.AppUserTeam.Update(appUserTeamDB);

                    var result = await _repository.SaveChangesAsync();

                    if (!result)
                        // return Success(null, null);
                        return Error("Error updating appUserTeam");
                }
            }
            catch (Exception ex)
            {
                return Error("Something broke" + ex.Message);
            }
            return Success("AppUserTeam Saved successfully");
        }


        public async Task<IActionResult> EditAppUserTeam(Guid clientId)

        {
            try
            {
                var appUserTeamEntity = await _repository.AppUserTeam.FindByCondition(x => x.Id == clientId).FirstOrDefaultAsync();
                if (appUserTeamEntity  == null)
                {
                    return NotFound();
                }

                var appUserTeamViewModel = new AppUserTeamVm
                {
                    Id = appUserTeamEntity.Id,
                   
                };

                ViewBag.Title = "Edit AppUserTeam";
                return View("_AppUserTeam", appUserTeamViewModel);
            }
            catch (Exception ex)
            {
                return Error("Something broke" + ex.Message);
            }
        }

        public async Task<IActionResult> DeleteAppUserTeam(Guid id)
        {
            try
            {
                var appUserTeamEntity = await _repository.AppUserTeam.FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
                if (appUserTeamEntity == null)
                {
                    return NotFound();
                }

                _repository.AppUserTeam.Delete(appUserTeamEntity);
                var result = await _repository.SaveChangesAsync();

                if (!result)
                {
                    return Error("Error deleting appUserTeam");
                }

                return Success("AppUserTeam deleted successfully");
            }
            catch (Exception ex)
            {
                return Error("Something broke" + ex.Message);
            }
        }

        #endregion

        #region Team
        public IActionResult Team()
        { 
            return View();
        }

        public async Task<IActionResult> CreateTeam(Guid teamId)
        {
            ViewBag.Title = "Create Team";

            var data=await _repository.User.GetAllAsync();

            var teamViewModel = new TeamVm
            {
                Supervisors = data.ToList(),
            };

            if (teamId != Guid.Empty)
            {
                var teamEntity = await _repository.Team.FindByCondition(x => x.Id == teamId).FirstOrDefaultAsync();

                if (teamEntity == null)
                {
                    return NotFound();
                }

                teamViewModel = new TeamVm
                {
                    Id = teamEntity.Id,
                    Name = teamEntity.Name,
                    NotificationType = teamEntity.NotificationType,
                    Supervisors = data.ToList(),
                    SupervisorId = teamEntity.AppUserId
                };

                ViewBag.Title = "Edit Team";
            }

            return View("_Team", teamViewModel);
        }

        public async Task<IActionResult> AssignTeam(Guid teamId)
        {
            var teamEntity = await _repository.Team.FindByCondition(x => x.Id == teamId).FirstOrDefaultAsync();

            var data = await _repository.User.GetAllAsync();

            return View("AppUserTeam", new AppUserTeamVm
            {
                Id = teamEntity.Id,
                AppUsers = data.ToList(),
            });
        }

        [HttpPost]
        public async Task<IActionResult> GetTeam(IDataTablesRequest dtRequest)
        {

            try
            {
                var data = await _repository.Team.FindAll().Include(x=>x.AppUser).ToListAsync();

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
        public async Task<IActionResult> SaveTeam(TeamVm team)
        {
            if (!ModelState.IsValid  && ModelState.ErrorCount >1)
                return Error("Validation error!, please check your data.");


            try
            {
                if (team.Id == Guid.Empty)
                {
                    var teamEntity = new Team
                    {
                        Name = team.Name,
                        NotificationType = team.NotificationType,
                        AppUserId = team.SupervisorId,
                    };

                    _repository.Team.Create(teamEntity);


                    var result = await _repository.SaveChangesAsync();

                    if (!result)
                        return Error("Error Creating Team!");
                }
                else
                {
                    var teamDB = await _repository.Team.FindByCondition(x => x.Id == team.Id).FirstOrDefaultAsync();

                    if (teamDB == null)
                    {
                        return Error("Team not found");
                    }


                    team.Name = team.Name;
                    team.NotificationType= team.NotificationType;

                    _repository.Team.Update(teamDB);

                    var result = await _repository.SaveChangesAsync();

                    if (!result)
                        // return Success(null, null);
                        return Error("Error updating team");
                }
            }
            catch (Exception ex)
            {
                return Error("Something broke" + ex.Message);
            }
            return Success("Team Saved successfully");
        }

        public async Task<IActionResult> GetTeamUsers(Guid id)
        {
            var team = await _repository.Team.FindByCondition(x => x.Id == id).Include(x => x.AppUserTeams).FirstOrDefaultAsync();

            var users = await _repository.User.GetAllAsync();

            var obj = new AppUserVM
            {
                Users=users.ToList(),
                TeamUsers = team.AppUserTeams.ToList(),
            };

            return Success("Fetched Successfully", obj);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeamUser(IFormCollection formData)
        {
            string userId = formData["UserId"];

            string teamId = formData["TeamId"];

            // await _repository.User.FindByCondition(x => x.Id == userId).FirstOrDefaultAsync();

            var appTeam = new AppUserTeam { AppUserId = userId, TeamId = Guid.Parse(teamId) };

            _repository.AppUserTeam.Create(appTeam);

            var status = await _repository.SaveChangesAsync();

            if (status)
                return Success("Created Successfully");

            return Error("Unable to Create");
        }

        public async Task<IActionResult> DeleteTeam(Guid id)
        {
            try
            {
                var teamEntity = await _repository.Team.FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
                if (teamEntity == null)
                {
                    return NotFound();
                }

                _repository.Team.Delete(teamEntity);
                var result = await _repository.SaveChangesAsync();

                if (!result)
                {
                    return Error("Error deleting team");
                }

                return Success("Team deleted successfully");
            }
            catch (Exception ex)
            {
                return Error("Something broke" + ex.Message);
            }
        }
        #endregion

        #region Intention
        public IActionResult Intention()
        {
            ViewBag.PageTitle = "Intention";
            return View();
        }

        public IActionResult CreateIntention()
        {
            ViewBag.Title = "Create intention";

            return View("_Intention");
        }

        [HttpPost]
        public async Task<IActionResult> GetIntention(IDataTablesRequest dtRequest)
        {

            try
            {
                var data = await _repository.Itention.GetAllAsync();

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
        public async Task<IActionResult> SaveIntention(IntentionVm intention)
        {
            if (!ModelState.IsValid  && ModelState.ErrorCount >1)
                return Error("Validation error!, please check your data.");


            try
            {
                if (intention.Id == Guid.Empty)
                {
                    var intentionEntity = new Intention
                    {
                        Name = intention.Name,
                        Kiswahili = intention.Kiswahili,
                    };

                    _repository.Itention.Create(intentionEntity);


                    var result = await _repository.SaveChangesAsync();

                    if (!result)
                        return Error("Error Creating Intention!");
                }
                else
                {
                    var intentionDB = await _repository.Itention.FindByCondition(x => x.Id == intention.Id).FirstOrDefaultAsync();

                    if (intentionDB == null)
                    {
                        return Error("Intention not found");
                    }


                    intentionDB.Name = intention.Name;
                    intentionDB.Kiswahili = intention.Kiswahili;

                    _repository.Itention.Update(intentionDB);

                    var result = await _repository.SaveChangesAsync();

                    if (!result)
                        // return Success(null, null);
                        return Error("Error updating intention");
                }
            }
            catch (Exception ex)
            {
                return Error("Something broke" + ex.Message);
            }
            return Success("Intention Saved successfully");
        }

        public async Task<IActionResult> EditIntention(Guid clientId)
        {
            try
            {
                var intentionEntity = await _repository.Itention.FindByCondition(x => x.Id == clientId).FirstOrDefaultAsync();

                if (intentionEntity == null)
                {
                    return NotFound();
                }

                var intentionViewModel = new IntentionVm
                {
                    Id = intentionEntity.Id,
                    Name= intentionEntity.Name,
                    Kiswahili=intentionEntity.Kiswahili,

                };
                ViewBag.Title = "Edit Intention";

                return View("_Intention", intentionViewModel);


            }
            catch (Exception ex)
            {
                return Error("Something broke" + ex.Message);
            }
        }
        public async Task<IActionResult> DeleteIntention(Guid id)
        {
            try
            {
                var intentionEntity = await _repository.Itention.FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
                if (intentionEntity == null)
                {
                    return NotFound();
                }

                _repository.Itention.Delete(intentionEntity);
                var result = await _repository.SaveChangesAsync();

                if (!result)
                {
                    return Error("Error deleting intention");
                }

                return Success("Intention deleted successfully");
            }
            catch (Exception ex)
            {
                return Error("Something broke" + ex.Message);
            }
        }


        #endregion

        #region SubIntention
        public IActionResult SubIntention()
        {
            ViewBag.PageTitle = "Sub Intention";
            return View();
        }

        public async Task<IActionResult> CreateSubIntention()
        {

            var intentions = await _repository.Itention.GetAllAsync();

            ViewBag.Intentions = intentions;

            return View("_SubIntention");

        }

        [HttpPost]
        public async Task<IActionResult> GetSubIntention(IDataTablesRequest dtRequest)
        {

            try
            {
                var data = await _repository.SubIntention.GetAllAsync();

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
        public async Task<IActionResult> SaveSubIntention(SubIntentionVm subintention)
        {
            if (!ModelState.IsValid  && ModelState.ErrorCount >1)
                return Error("Validation error!, please check your data.");

            try
            {

                var intentionEntity = await _repository.Itention.GetByIdAsync(subintention.ItentionId);

                if (intentionEntity == null)
                {
                    ModelState.AddModelError("ItentionId", "Intention not found");
                    return View("_SubIntention", subintention);
                }

                if (subintention.Id == Guid.Empty)
                {
                    var subintentionEntity = new SubIntention
                    {
                        Name = subintention.Name,
                        IntentionId = subintention.ItentionId,
                        Kiswahili = subintention.Kiswahili,
                    };

                    _repository.SubIntention.Create(subintentionEntity);


                }
                else
                {
                    var subintentionDB = await _repository.SubIntention.FindByCondition(x => x.Id == subintention.Id).FirstOrDefaultAsync();

                    if (subintentionDB == null)
                    {
                        ModelState.AddModelError("Id", "SubIntention not found");
                        return View("_SubIntention", subintention);

                    }

                    subintentionDB.Name = subintention.Name;
                    subintentionDB.IntentionId = subintention.ItentionId;
                    subintentionDB.Kiswahili = subintention.Kiswahili;


                    _repository.SubIntention.Update(subintentionDB);
                }

                var result = await _repository.SaveChangesAsync();

                if (result)
                {
               
                    return Success("SubIntention saved Successfully");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ServerError", "Something broke: " + ex.Message);
                return Error("There was an error creating subIntention");
            }

            return Error("Sorry SubIntention was not created");
        }

        public async Task<IActionResult> EditSubIntention(SubIntentionVm subintention)
        {
            try
            {

                var subintentionEntity = await _repository.SubIntention
                    .FindByCondition(x => x.Id == subintention.Id)
                    .Include(x => x.Intention)
                    .FirstOrDefaultAsync();

                if (subintentionEntity == null)
                {
                    return NotFound();
                }

                var intentions = await _repository.Itention.GetAllAsync(); 


                var subintentionViewModel = new SubIntentionVm
                {
                    Id = subintentionEntity.Id,
                    Name= subintentionEntity.Name,
                    ItentionId = subintentionEntity.IntentionId,
                    Kiswahili = subintentionEntity.Kiswahili,
                    Intentions=intentions.ToList()
                };
                ViewBag.Title = "Edit SubIntention";

                return View("_SubIntention", subintentionViewModel);


            }
            catch (Exception ex)
            {
                return Error("Something broke" + ex.Message);
            }
        }


        public async Task<IActionResult> DeleteSubIntention(Guid id)
        {
            try
            {
                var subintentionEntity = await _repository.SubIntention.FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
                if (subintentionEntity == null)
                {
                    return NotFound();
                }

                _repository.SubIntention.Delete(subintentionEntity);
                var result = await _repository.SaveChangesAsync();

                if (!result)
                {
                    return Error("Error deleting subintention");
                }

                return Success("Subintention deleted successfully");
            }
            catch (Exception ex)
            {
                return Error("Something broke" + ex.Message);
            }
        }


        #endregion

    }
}

