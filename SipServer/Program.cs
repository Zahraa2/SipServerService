using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SIPServer.Call;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using SIPServer;
using Microsoft.Extensions.Configuration.Json;
namespace SipServer
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static IServiceProvider serviceProvider;
        public static IConfiguration Configuration { get; private set; }
        static void Main()
        {
            //var services = new ServiceCollection();
            //ConfigureServices(services);
            //serviceProvider = services.BuildServiceProvider();
            //Service1 myService = new Service1();
            
            MainThread.Start();

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Service1()
            };
            ServiceBase.Run(ServicesToRun);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Build configuration
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = configurationBuilder.Build();

            // Register configuration
            services.AddSingleton<IConfiguration>(Configuration);

            // Register other services
            // services.AddTransient<IMyService, MyService>();
            services.AddTransient<CallManager>();
            services.AddTransient<SpeechToText>();
            services.AddTransient<Chatbot>();
            services.AddTransient<TextToSpeech>();

            services.AddSingleton<Server>();

        }
    }
}
