using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedCrossChat.Dialogs;

namespace RedCrossChat.Extensions
{
    public static class ServicesExtensions
    {
       

        public static void ConfigureDialogs(this IServiceCollection services, IServiceCollection services2)
        {
            services.AddSingleton<CounselorDialog>();

            services.AddTransient<PersonalDialog>();

            services.AddSingleton<AwarenessDialog>();

            services.AddSingleton<BreathingDialog>();

            services.AddTransient<AiDialog>();

            services.AddSingleton<ChatGpt>();

            // The MainDialog that will be run by the bot.
            services.AddTransient<MainDialog>();
        }
    }
}
