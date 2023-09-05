using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedCrossChat.Dialogs;

namespace RedCrossChat.Extensions
{
    public static class ServicesExtensions
    {
       

        public static void ConfigureDialogs(this IServiceCollection services)
        {
            services.AddSingleton<CounselorDialog>();

            services.AddSingleton<PersonalDialog>();

            services.AddSingleton<AwarenessDialog>();

            services.AddSingleton<BreathingDialog>();

            services.AddSingleton<AiDialog>();

            services.AddSingleton<ChatGpt>();

            // The MainDialog that will be run by the bot.
            services.AddSingleton<MainDialog>();
        }
    }
}
