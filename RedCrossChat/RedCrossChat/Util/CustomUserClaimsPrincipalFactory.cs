using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using RedCrossChat.Entities.Auth;
using RedCrossChat.Entities;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RedCrossChat
{
    public class CustomUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<AppUser, AppRole>
    {
        public CustomUserClaimsPrincipalFactory(
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(AppUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            identity.AddClaim(new Claim("UserId", user.Id));
            
            return identity;
        }
    }
}
