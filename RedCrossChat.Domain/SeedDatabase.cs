using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RedCrossChat.Entities.Auth;
using RedCrossChat.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RedCrossChat.Domain
{
    public class SeedDatabase
    {
        public static async Task SeedInitialData(
            AppDBContext context,
           RoleManager<AppRole> roleMgr,
           UserManager<AppUser> userMgr)
        {
            var superAdminRole = new AppRole(Constants.SuperAdministratorId, "Super Administrator", true, DateTime.Now);
            var adminRole = new AppRole(Constants.AdministratorId, "Administrator", true, DateTime.Now);

            if (!context.Roles.Any())
            {
                // Create roles
                roleMgr.CreateAsync(superAdminRole).GetAwaiter().GetResult();
                roleMgr.CreateAsync(adminRole).GetAwaiter().GetResult();

                #region Assign claims to super admin
                // Get all claims to assign to super user admin
                var existingRoleClaims = await roleMgr.GetClaimsAsync(superAdminRole);
                if (existingRoleClaims.Count() == 0)
                {
                    // Get all claims
                    var appClaims = await context.AppClaims.ToListAsync();
                    if (appClaims.Count() > 0)
                    {
                        foreach (var appClaim in appClaims)
                        {
                            var result = await roleMgr.AddClaimAsync(superAdminRole, new Claim(appClaim.Name, appClaim.Name));
                            if (result.Succeeded) { }
                        }
                    }
                }
                #endregion
            }

            if (!context.Users.Any(x => x.UserName.ToLower() == "admin@redcross.com"))
            {
                // Create admin user
                var adminUser = new AppUser
                {
                    Id = "8bd49ae1-6059-4099-8002-9bdaea92ced3",
                    FirstName = "Admin",
                    LastName = "User",
                    UserName = "admin@redcross.com",
                    Email = "admin@redcross.com",

                };
                userMgr.CreateAsync(adminUser, "Test@!23").GetAwaiter().GetResult();

                // Add role to user
                userMgr.AddToRoleAsync(adminUser, superAdminRole.Name).GetAwaiter().GetResult();
            }
        }

        public static async Task SeedAppModules(AppDBContext context)
        {
            if (!context.AppModules.Any())
            {
                var appModules = await SeedHelper.GetSeedData<AppModule>("AppModules.json");
                foreach (var appModule in appModules.OrderBy(x => x.Id))
                {
                    var am = new AppModule
                    {
                        //Id = appModule.Id,
                        Name = appModule.Name,
                        Description = appModule.Description
                    };
                    context.AppModules.Add(am);
                    context.SaveChanges();
                }

                //await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.ElisaPlateDefs ON");
                //context.SaveChanges();
                //await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.ElisaPlateDefs OFF");
            }
        }

        public static async Task SeedAppClaims(AppDBContext context)
        {
            if (!context.AppClaims.Any())
            {
                var appClaims = await SeedHelper.GetSeedData<AppClaim>("AppClaims.json");
                foreach (var appClaim in appClaims.OrderBy(x => x.Id))
                {
                    var ac = new AppClaim
                    {
                        //Id = appClaim.Id,
                        Name = appClaim.Name,
                        Description = appClaim.Description,
                        AppModuleId = appClaim.AppModuleId
                    };
                    context.AppClaims.Add(ac);
                    context.SaveChanges();
                }
            }
        }
    }
}
