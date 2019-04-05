using System;
using System.IO;
using System.Threading;
using Google.Apis.PhotosLibrary.v1;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using google_photos_upload.Services;

namespace google_photos_upload
{
    class Program
    {
        static void Main(string[] args)
        {
            // Dependency Injection - create a new ServiceCollection 
            var serviceCollection = new ServiceCollection();
            IServiceProvider serviceProvider = ConfigureServices(serviceCollection);

            // Instantiate the App Runner class
            var app = serviceProvider.GetRequiredService<App>();
            app.DoAction(args);

            // Nlog shutdown - ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
            NLog.LogManager.Shutdown();
        }

        /// <summary>
        /// Configure services for Dependency Injection
        /// </summary>
        /// <param name="serviceCollection">Dependency Injection ServiceCollection instance</param>
        /// <returns>IServiceProvider instance</returns>
        private static IServiceProvider ConfigureServices(IServiceCollection serviceCollection)
        {
            return serviceCollection
                //Add logging configuration
                .AddLogging(builder =>
                {
                    builder.SetMinimumLevel(LogLevel.Trace);
                    builder.AddNLog(new NLogProviderOptions
                    {
                        CaptureMessageTemplates = true,
                        CaptureMessageProperties = true
                    });
                })

                //Register the App Runner class
                .AddTransient<App>()

                //Register the App Service classes
                .AddSingleton<IUploadService, UploadService>()
                .AddSingleton<IAuthenticationService, AuthenticationService>()

                //Finally build the ServiceProvider and return it
                .BuildServiceProvider();
        }
    }
}
