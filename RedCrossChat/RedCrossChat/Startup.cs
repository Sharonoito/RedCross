using DataTables.AspNet.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RedCrossChat.Bots;
using RedCrossChat.Contracts;
using RedCrossChat.Dialogs;
using RedCrossChat.Domain;
using RedCrossChat.Entities;
using RedCrossChat.Extensions;
using RedCrossChat.Repository;

namespace RedCrossChat
{
    public class Startup(IConfiguration configuration)
    {

        private readonly IConfiguration _config = configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
           // services.AddHttpClient().AddControllers().AddNewtonsoftJson();


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

            services.RegisterDataTables();

            // Create the Bot Framework Authentication to be used with the Bot Adapter.
            services.AddSingleton<BotFrameworkAuthentication, ConfigurationBotFrameworkAuthentication>();

            // Create the Bot Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            // Create the storage we'll be using for User and Conversation state. (Memory is great for testing purposes.)
            services.AddSingleton<IStorage, MemoryStorage>();

            var connectionString = _config.GetConnectionString("LocalConnection");

            //Use sql Server Conversations
            services.AddDbContextPool<AppDBContext>(options =>
                options.UseSqlServer(connectionString));

            //Repositoty for database management
            services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();

            // Create the User state. (Used in this bot's Dialog implementation.)
            services.AddSingleton<UserState>();

            // Create the Conversation state. (Used by the Dialog system itself.)
            services.AddSingleton<ConversationState>();

            // Register LUIS recognizer
            services.AddSingleton<FlightBookingRecognizer>();

            // Register dialogs used in the bot Project
            services.ConfigureDialogs(services);

            //Testing the dialog set manager
            //services.AddSingleton(new DialogSet());

            services.ConfigureClaimBasedAuthorization();

            //services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddTransient<IBot, DialogAndWelcomeBot<MainDialog>>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

             
                await SeedUsers.IFetch(app.ApplicationServices);
            }

            app//.UseDefaultFiles()
                .UseSentryTracing()
                .UseStaticFiles()
                .UseWebSockets()
                .UseRouting()
                .UseAuthorization()
                .UseAuthentication()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();

                    endpoints.MapControllerRoute(name:"default",pattern: "{controller=Home}/{action=Index}/{id?}");
                });

            // app.UseHttpsRedirection();
        }
    }
}
