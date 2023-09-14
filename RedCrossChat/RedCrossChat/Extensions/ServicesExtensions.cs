using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedCrossChat.Dialogs;
using RedCrossChat.Domain;
using RedCrossChat.Entities.Auth;
using RedCrossChat.Entities;
using System;

namespace RedCrossChat.Extensions
{
    public static class ServicesExtensions
    {


        public static void ConfigureDialogs(this IServiceCollection services, IServiceCollection services2)
        {
            services.AddSingleton<CounselorDialog>();

            services.AddTransient<PersonalDialog>();

            services.AddTransient<AwarenessDialog>();

            services.AddTransient<BreathingDialog>();

            services.AddTransient<AiDialog>();

            services.AddSingleton<ChatGpt>();

            // The MainDialog that will be run by the bot.
            services.AddTransient<MainDialog>();
        }

        public static void ConfigureIdentity(this IServiceCollection services)
        {
            services.AddIdentity<AppUser, AppRole>()
                .AddEntityFrameworkStores<AppDBContext>()
                .AddDefaultTokenProviders();

            // Configure password reset token to be valid for 1 hour
            services.Configure<DataProtectionTokenProviderOptions>(
                opt => opt.TokenLifespan = TimeSpan.FromHours(1));
        }

        public static void ConfigureClaimBasedAuthorization(this IServiceCollection services)
        {
            // Create claims policy to be used in controller or action level
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ManageAuthPolicy", policy => policy.RequireClaim("Manage Users"));
                // RoleClaims
                options.AddPolicy("ManageRoleClaimsPolicy", policy => policy.RequireClaim("Manage Role Claims"));
                // Roles
                options.AddPolicy("ManageRolesPolicy", policy => policy.RequireClaim("Manage Roles"));
                // Users
                options.AddPolicy("ManageUsersPolicy", policy => policy.RequireClaim("Manage Users"));

            });
        }
    }
}
