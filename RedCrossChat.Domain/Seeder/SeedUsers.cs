using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RedCrossChat;
using RedCrossChat.Entities;
using RedCrossChat.Entities.Auth;
using System.Security.Claims;

namespace RedCrossChat.Domain
{
    public static class SeedUsers
    { 
        public static async Task IFetch(IServiceProvider serviceProvider)
        {

            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDBContext>();
                // Use dbContext within this scope

                var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

                var superAdminRole = new AppRole(Constants.SuperAdministratorId, Constants.SuperAdministrator, true, DateTime.Now);
                var adminRole = new AppRole(Constants.AdministratorId, Constants.Administrator, true, DateTime.Now);

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

                       var appClaims= context.AppClaims.ToListAsync();

                       var appClaimCount =  context.AppClaims.Count();

                        
                        if (appClaims.Result.Count > 0)
                        {
                            foreach (var appClaim in appClaims.Result)
                            {
                                var result = await roleMgr.AddClaimAsync(superAdminRole, new Claim(appClaim.Name, appClaim.Name));
                                if (result.Succeeded) { }
                            }
                        }
                    }
                    #endregion
                }

                if (!context.Users.Any(x => x.UserName.ToLower() == Constants.DefaultSuperAdminEmail))
                {
                    // Create admin user
                    var adminUser = new AppUser
                    {
                        Id = "8bd49ae1-6059-4099-8002-9bdaea92ced3",
                        FirstName = "Admin",
                        LastName = "User",
                        UserName = Constants.DefaultSuperAdminEmail,
                        Email = Constants.DefaultSuperAdminEmail,
                    };
                    userMgr.CreateAsync(adminUser, "Test@!23").GetAwaiter().GetResult();

                    // Add role to user
                    userMgr.AddToRoleAsync(adminUser, superAdminRole.Name).GetAwaiter().GetResult();
                }
                if (!context.County.Any())
                {
                    var data = await SeedHelper.GetSeedData<DBCounty>("County.json");

                    foreach (var item in data)
                    {
                        context.County.Add(new DBCounty { Name = item.Name, prefix = "SM", Code = item.Code });

                        await context.SaveChangesAsync();
                    }


                }

                if (!context.AgeBand.Any())
                {
                    var data = await SeedHelper.GetSeedData<AgeBand>("AgeBand.json");

                    foreach (var item in data)
                    {
                        context.AgeBand.Add(new AgeBand { Name = item.Name, Kiswahili = item.Kiswahili, Highest = item.Highest, Lowest = item.Lowest });

                        await context.SaveChangesAsync();
                    }
                }

                if (!context.Gender.Any())
                {
                    var data = await SeedHelper.GetSeedData<AgeBand>("Gender.json");

                    foreach (var item in data)
                    {

                        context.Gender.Add(new Gender { Name = item.Name, Kiswahili = item.Kiswahili });

                        await context.SaveChangesAsync();

                    }
                }

                if (!context.Feeling.Any())
                {
                    var data = await SeedHelper.GetSeedData<DBFeeling>("Feelings.json");

                    foreach (var feeling in data)
                    {

                        context.Feeling.Add(new DBFeeling { Name = feeling.Name, Description = feeling.Description, Synonymns = feeling.Synonymns, Kiswahili = feeling.Kiswahili });

                        await context.SaveChangesAsync();
                    }
                }

                if (!context.Profession.Any())
                {
                    var data = await SeedHelper.GetSeedData<Profession>("Profession.json");

                    foreach (var item in data)
                    {
                        context.Profession.Add(new Profession { Name = item.Name, Kiswahili = item.Kiswahili, Synonyms = "" });

                        await context.SaveChangesAsync();
                    }
                }

                if (!context.MaritalState.Any())
                {
                    var data = await SeedHelper.GetSeedData<MaritalState>("MaritalStatus.json");

                    foreach (var item in data)
                    {
                        context.MaritalState.Add(new MaritalState { Name = item.Name, Kiswahili = item.Kiswahili, Synonyms = "" });

                        await context.SaveChangesAsync();
                    }
                }

                if (!context.Question.Any())
                {
                    var data = await SeedHelper.GetSeedData<Question>("Questions.json");

                    foreach (var item in data)
                    {
                        context.Question.Add(new Question { Kiswahili=item.Kiswahili,question=item.question,Code=item.Code });

                        await context.SaveChangesAsync();
                    }
                }

                if (!context.Exercise.Any())
                {
                    var data = await SeedHelper.GetSeedData<Exercise>("Exercise.json");

                    foreach(var item in data)
                    {
                        context.Exercise.Add(new Exercise { Feeling=item.Feeling,Value=item.Value,Exercises=item.Exercises,Kiswahili=item.Kiswahili });

                        await context.SaveChangesAsync();
                    }

                }

            }

        }
    }
}
