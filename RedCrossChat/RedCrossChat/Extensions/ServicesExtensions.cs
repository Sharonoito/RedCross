using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedCrossChat.Dialogs;
using RedCrossChat.Domain;
using RedCrossChat.Entities.Auth;
using RedCrossChat.Entities;
using System;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector.Authentication;
using RedCrossChat.Contracts;
using RedCrossChat.Repository;

namespace RedCrossChat.Extensions
{
    public static class ServicesExtensions
    {


        public static void ConfigureDialogs(this IServiceCollection services)
        {
            services.AddSingleton<CounselorDialog>();

            services.AddTransient<PersonalDialog>();

            services.AddTransient<AwarenessDialog>();

            services.AddTransient<BreathingDialog>();

            services.AddTransient<AiDialog>();

            services.AddSingleton<ChatGpt>();

            services.AddTransient<LanguageSelectionDialog>();

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

        public static void ConfigureBotServices(this IServiceCollection services)
        {
            services.AddSingleton<BotFrameworkAuthentication, ConfigurationBotFrameworkAuthentication>();

            // Create the Bot Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            //Repositoty for database management
            services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();

            // Create the User state. (Used in this bot's Dialog implementation.)
            services.AddSingleton<UserState>();

            // Create the Conversation state. (Used by the Dialog system itself.)
            services.AddSingleton<ConversationState>();

            // Register LUIS recognizer
            services.AddSingleton<FlightBookingRecognizer>();
        }

        public static void ConfigureServicesMvcAndAuthentication(this IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);


            services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddRazorPages().AddRazorRuntimeCompilation();

            services.AddScoped<IUserClaimsPrincipalFactory<AppUser>, CustomUserClaimsPrincipalFactory>();


            services.ConfigureIdentity();


            services.Configure<IdentityOptions>(o =>
            {
                o.Password.RequireDigit = true;
                o.Password.RequiredLength = 6;
                o.Password.RequireUppercase = true;
                o.Password.RequireLowercase = true;
                o.Password.RequireNonAlphanumeric = true;
            });

            // Change Login URl
            services.ConfigureApplicationCookie(o =>
            {
                o.LoginPath = "/Auth/Login";
                o.AccessDeniedPath = new Microsoft.AspNetCore.Http.PathString("/Error/Error");
            });

            services.AddMvc(options =>
            {
                var policy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter(policy));
                options.EnableEndpointRouting = true;
            }).AddXmlSerializerFormatters();
        }
    }
}
