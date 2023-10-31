using DataTables.AspNet.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RedCrossChat.Bots;
using RedCrossChat.Dialogs;
using RedCrossChat.Domain;
using RedCrossChat.Extensions;

namespace RedCrossChat
{
    public class Startup
    {

        private readonly IConfiguration _config;

        public IWebHostEnvironment HostingEnvironment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _config = configuration;
            HostingEnvironment = environment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {          
            services.ConfigureServicesMvcAndAuthentication();


            services.RegisterDataTables();

            // Create the storage we'll be using for User and Conversation state. (Memory is great for testing purposes.)
            services.AddSingleton<IStorage, MemoryStorage>();

            var connectionString = _config.GetConnectionString(HostingEnvironment.IsDevelopment() ?  "DefaultConnection": "LocalConnection");
            //var connectionString = _config.GetConnectionString("LocalConnection");


            //Use sql Server Conversations
            services.AddDbContextPool<AppDBContext>(options =>
                options.UseSqlServer(connectionString));

            // Create the Bot Framework Authentication to be used with the Bot Adapter.
            services.ConfigureBotServices();

            // Register dialogs used in the bot Project
            services.ConfigureDialogs();


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
