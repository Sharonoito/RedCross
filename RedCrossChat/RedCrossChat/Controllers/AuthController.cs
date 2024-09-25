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
using RedCrossChat.ViewModel;
using RedCrossChat.Extensions;
using System.Text;

namespace RedCrossChat.Controllers
{
    public class AuthController : BaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager ;
        private readonly RoleManager<AppRole> _roleManager ;

        private readonly IRepositoryWrapper _repository ;

        public AuthController(
            UserManager<AppUser> userManager,
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


        #region ResetPassword

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string returnUrl = null)
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPasswordOTP(ResetViewModel resetViewModel)
        {
            AppUser user = await _userManager.FindByEmailAsync(resetViewModel.Email);

            if (user == null)
            {
                return Error("Not found in the system");
            }

            // Build the email body
            if (user.LastOTP == 0)
            {
                user.LastOTP = new Random().Next(1000, 9999); // Generate a 6-digit OTP

                await _userManager.UpdateAsync(user); // Save the OTP to the database
            }

            // Build the email body
            StringBuilder body = new StringBuilder();
            body.AppendLine($"Dear {user.FullName},");
            body.AppendLine();
            body.AppendLine("We received a request to reset your password.");
            body.AppendLine("Please use the OTP below to complete your password reset:");
            body.AppendLine();
            body.AppendLine(user.LastOTP.ToString()); // Include the OTP
            body.AppendLine();
            body.AppendLine("If you did not request a password reset, please ignore this email.");
            body.AppendLine();
            body.AppendLine("Thank you,");
            body.AppendLine("RedCross Chatbot Team");

            // Send the email using SMTP
            bool response = await SmptMailExtension.SendEmailAsync(resetViewModel.Email, "RedCross Chatbot | Reset Password", body.ToString());

            if (response)
            {
                return Success("Otp Sent Successfully");
            }

            return Error("Unable to Send OTP");
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPasswordVerify(ResetViewModel resetViewModel)
        {
            var user=await _userManager.FindByEmailAsync(resetViewModel.Email);

            if (user != null) { 

                if(user.LastOTP.ToString() == resetViewModel.OTP)
                {
                    String response= await _userManager.GeneratePasswordResetTokenAsync(user);

                    resetViewModel.Token = response;

                    return Success("Validated Successfully",resetViewModel);
                }
            }

            return Error("This token is invalid");
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetConfirmPassword(ResetViewModel resetViewModel)
        {

            var user = await _userManager.FindByEmailAsync(resetViewModel.Email);

            if (user != null)
            {

                if (user.LastOTP.ToString() == resetViewModel.OTP)
                {

                    await _userManager.ResetPasswordAsync(user,resetViewModel.Token,resetViewModel.Password);

                    user.LastOTP= new Random().Next(1000, 9999);

                    await _userManager.UpdateAsync(user);

                    return Success("Validated Successfully");
                }
            }

            return Error("Unable to update password");
        }
        
        #endregion 

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


                var ipAddress = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();

                if (string.IsNullOrEmpty(ipAddress))
                {
                    ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                }

                var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

              

                if (result.Succeeded)
                {
                    _repository.AuthLoginLog.Create(new AuthLoginLog()
                    {
                        AppUserId = user.Id,
                        IpAddress = ipAddress,
                        UserAgent = userAgent,

                    });

                    await _repository.SaveChangesAsync();

                    return RedirectToLocal(returnUrl);

                }
                   
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
            await _signInManager.SignOutAsync();

            // Sign out using a generic context - makes sure it signs out all 
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }

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
                  //  .GetAllAsync();
                    .FindByCondition(x=>x.IsDeactivated ==false).ToListAsync();
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
        [HttpPost]
        public async Task<IActionResult> GetLogs(IDataTablesRequest dtRequest)
        {
            try
            {

               var data= await _repository.AuthLoginLog.FindByCondition(x=>x.AppUser !=null).Include(x=>x.AppUser).ToListAsync();

               /* var data = await _repository.AuthLoginLog
                    
                      .GetAllAsync();*/
                   // .FindByCondition(x => x.IsDeactivated == false).ToListAsync();
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
                    AppUser appUser = new AppUser
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        UserName = user.Email,
                        NormalizedEmail = user.Email.ToUpper(),
                        NormalizedUserName = user.Email.ToUpper()
                    };
                    
                    appUser.LastOTP = new Random().Next(1000, 9999); // Generate a 6-digit OTP
                        

                    var result = await _userManager.CreateAsync(appUser, "Test@!23");
/*
                    if (user.RoleNames != null)
                    {
                        IdentityResult addResult = await _userManager.AddToRolesAsync(appUser, user.RoleNames);
                        if (!addResult.Succeeded)
                            return Error("Failed to assign user roles.");
                    }*/

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
