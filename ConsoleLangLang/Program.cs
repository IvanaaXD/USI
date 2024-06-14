using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using LangLang.Data;
using LangLang.Controller;
using Microsoft.EntityFrameworkCore;

namespace ConsoleLangLang
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

            try
            {
                MainConsole.Display(); 
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred during application execution.");
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(configure => configure.AddConsole());

            services.AddDbContext<AppDbContext>(options =>
            {
                var connectionString = "Host=localhost;Database=usi_data;Username=postgres;Password=1234";
                options.UseNpgsql(connectionString); 
            });

            services.AddTransient<DirectorController>();
            services.AddTransient<CourseController>();
            services.AddTransient<ExamTermController>();
            services.AddTransient<MainConsole>();
        }


    }
}
