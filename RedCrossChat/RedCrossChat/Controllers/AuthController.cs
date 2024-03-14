using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using RedCrossChat.Contracts;
using RedCrossChat.Entities.Auth;
using RedCrossChat.Entities;
using System.Threading.Tasks;
using System;
using System.Linq;
using DataTables.AspNet.Core;
using DataTables.AspNet.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using RedCrossChat.Domain;

namespace RedCrossChat.Controllers
{
    public class AuthController : BaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager ;
        private readonly RoleManager<AppRole> _roleManager ;

        private readonly IRepositoryWrapper _repository ;

        public AuthController(UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        RoleManager<AppRole> roleManager,
        IRepositoryWrapper repository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _repository = repository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginVM()
            {
                ReturnUrl = returnUrl
            });
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginVM model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Sign out any previous sessions
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null && !user.IsDeactivated)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, true, false);

                if (result.Succeeded)
                    return RedirectToLocal(returnUrl);
            }

            ModelState.AddModelError("", "Invalid username or password!");
            return View(new LoginVM()
            {
                ReturnUrl = returnUrl
            });
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {

            return RedirectToAction(nameof(HomeController.Index), "Home");

        }


        [AllowAnonymous]
        public async Task<IActionResult> DeactivateAccount(Guid clientId)
        {
            var user = await _userManager.FindByIdAsync(clientId.ToString());

            if (user != null)
            {
                user.IsDeactivated = true;

                await _userManager.UpdateAsync(user);

                return PartialView("_AccountDeactivated");
            }
            return Json(new { success = false, message = "Account deactivation failed. User not found." });
        }


        public IActionResult Profile()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut()
        {
            //Sign out using the specific identity
            //await _signInManager.SignOutAsync();

            // Sign out using a generic context - makes sure it signs out all 
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

            return RedirectToAction("login", "auth");
        }

        public IActionResult Users()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetUsers(IDataTablesRequest dtRequest)
        {
            try
            {
                var data = await _repository.User
                    .GetAllAsync();
                    //.FindByCondition(x=>x.Email !=Constants.DefaultSuperAdminEmail).ToListAsync();
                // Filter them

                var filteredRows = data
                    .AsQueryable()
                    .FilterBy(dtRequest.Search, dtRequest.Columns);

                // Sort and paginate them
                var pagedRows = filteredRows
                    .SortBy(dtRequest.Columns)
                    .Skip(dtRequest.Start)
                    .Take(dtRequest.Length);

                var response = DataTablesResponse.Create(dtRequest, data.Count(),
                    filteredRows.Count(), pagedRows);

                return new DataTablesJsonResult(response);
             //   return Ok("Success");

            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        public async Task<IActionResult> CreateUser(Guid clientId)
        {
            ViewBag.Title = "Create User";

            var userVm=new UserVm();

            var roles = _roleManager.Roles.Select(x => new SelectListItem { Text = x.Name, Value = x.Name }).ToList();

            if (clientId != Guid.Empty)
            {
                var user = await _repository.User.FindByCondition(x => x.Id == clientId.ToString()).FirstOrDefaultAsync();

                var roleNames = await _userManager.GetRolesAsync(user);
              
                userVm = new UserVm
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    RoleNames= roleNames.Cast<string>().ToArray(),
                    Id = clientId
                };
            }

            userVm.RoleList = roles;

            return View("_User", userVm);
        }

        public IActionResult ResetPassword()
        {

            return View("_ResetPassword");
        }

        public async Task< IActionResult> UpdatePassword(ProfileVm vm)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null) {

               var token= await _userManager.GeneratePasswordResetTokenAsync(user);

               var status=await _userManager.ResetPasswordAsync(user, token, vm.ConfirmPassword);

                if (status.Succeeded)
                {
                    return Success("Updated Successfully");
                }
                else
                {
                    return Error(status.Errors.First().Description);
                }

            }
            return Error("Something broke");
        }

      

        public async Task< IActionResult> SaveUser(UserVm user) {

            if (!ModelState.IsValid && ModelState.ErrorCount >1)
                return Error("Validation error!, please check your data.");

            try
            {
                if (user.Id == Guid.Empty)  //install instance
                {
                    var appUser = new AppUser
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        UserName = user.Email,
                        NormalizedEmail = user.Email.ToUpper(),
                        NormalizedUserName = user.Email.ToUpper()
                    };

                    var result = await _userManager.CreateAsync(appUser, "Test@!23");

                    if (user.RoleNames != null)
                    {
                        IdentityResult addResult = await _userManager.AddToRolesAsync(appUser, user.RoleNames);
                        if (!addResult.Succeeded)
                            return Error("Failed to assign user roles.");
                    }

                    if (!result.Succeeded)
                        return Error(result.Errors.First().Description.ToString());

                    return Success(null, null);
                }
                else
                {
                    var userDB = await _repository.User.FindByCondition(x => x.Id == user.Id.ToString()).FirstOrDefaultAsync();

                    userDB.FirstName = user.FirstName;
                    userDB.LastName = user.LastName;
              
                    userDB.PhoneNumber=user.PhoneNumber;

                    try
                    {
                        var currentRoles = await _userManager.GetRolesAsync(userDB);

                        _repository.User.Update(userDB);

                        if (currentRoles != null && currentRoles.Count > 0)
                        {
                            IdentityResult removeResult = await _userManager.RemoveFromRolesAsync(userDB, currentRoles.ToArray());
                            if (!removeResult.Succeeded)
                                return Error("Failed to remove user roles.");
                        }

                        // Assign the newly selected Roles
                        if (user.RoleNames != null)
                        {
                            IdentityResult addResult = await _userManager.AddToRolesAsync(userDB, user.RoleNames);

                            if (!addResult.Succeeded)
                                return Error("Failed to assign user roles.");
                        }

                        //var result = await _repository.SaveChangesAsync();

                        if (true)
                        {
                            return Success("Updated Successfully");
                        }
                    }
                    catch(Exception ex)
                    {
                        return Error(ex.ToString());
                    }
             

                    

                    //return Error("Unable to Update ", null);
                }
            }
            catch(Exception ex)
            {
                return Error(ex.Message);
            }

        }


        public IActionResult Roles()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> GetRoles(IDataTablesRequest dtRequest)
        {

            try
            {

                var data = await _repository.Role
                    .GetAllAsync();

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




    }
}
