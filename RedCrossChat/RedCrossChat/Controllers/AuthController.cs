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

namespace RedCrossChat.Controllers
{
    public class AuthController(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        RoleManager<AppRole> roleManager,
        IRepositoryWrapper repository) : BaseController
    {
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly SignInManager<AppUser> _signInManager = signInManager;
        private readonly RoleManager<AppRole> _roleManager = roleManager;

        private readonly IRepositoryWrapper _repository = repository;

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


            var result = await _signInManager
               .PasswordSignInAsync(model.Email, model.Password, true, false);

            if (result.Succeeded)
                return RedirectToLocal(returnUrl);
            else
            {
                ModelState.AddModelError("", "Invalid username or password!");
                return View(new LoginVM()
                {
                    ReturnUrl = returnUrl
                });
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {

            return RedirectToAction(nameof(HomeController.Index), "Home");

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
                var data = await _repository.User.GetAllAsync();
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

        public IActionResult CreateUser()
        {
            ViewBag.Title = "Create User";

            return View("_User");
        }

        public async Task<IActionResult> EditUser(Guid clientId)
        {

            await _repository.User.FindByCondition(x => x.Id == clientId.ToString()).FirstOrDefaultAsync();



            return View("_User");
        }

        public async Task< IActionResult> SaveUser(UserVm user) {

            if (!ModelState.IsValid)
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

                    if (!result.Succeeded)
                        return Error(result.Errors.First().Description.ToString());

                    return Success(null, null);
                }
            }
            catch(Exception)
            {
                return Error("Something broke");
            }


            return Error("No response");
        }


    }
}
