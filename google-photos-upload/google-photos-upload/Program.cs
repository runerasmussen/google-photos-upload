using System;
using System.IO;
using System.Threading;
using Google.Apis.PhotosLibrary.v1;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace google_photos_upload
{
    class Program
    {
        private static ILogger _logger = null;

        static void Main(string[] args)
        {
            var servicesProvider = BuildDi();

            _logger = servicesProvider.GetService<ILoggerFactory>().CreateLogger<Program>();

            DrawConsoleInterface();

            // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
            NLog.LogManager.Shutdown();
        }

        private static void DrawConsoleInterface()
        {
            Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
            Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
            Console.WriteLine();
            Console.WriteLine("                -- Google Photos Upload --");
            Console.WriteLine();
            Console.WriteLine("Unofficial upload utility for Google Photos");
            Console.WriteLine("User Guide: See GitHub");
            Console.WriteLine("https://github.com/runerasmussen/google-photos-upload");
            Console.WriteLine();
            Console.WriteLine("WARNING: This utility is provided as-is without any guarantees!!!");
            Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
            Console.WriteLine();
            Console.WriteLine();


            bool appexit = false;

            if (!UploadHandler.Initialize(_logger))
            {
                Console.WriteLine("Critical error occured - could not establish authentication with Google");
                Console.WriteLine("See log for details.");
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();

                appexit = true;
            }


            while (!appexit)
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("--------------------------------------------------------------------------------");
                Console.WriteLine("Options:");
                Console.WriteLine("1 - List current Google Photos Album");
                Console.WriteLine("2 - Upload Single Folder into Google Photos as an Album ");
                Console.WriteLine("3 - Upload Multiple Folders from a main Folder into Google Photos as Albums");
                Console.WriteLine("Press any other key to close the program");
                Console.Write("Type your number of choice: ");

                short userchoice = GetUserChoice();

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("--------------------------------------------------------------------------------");

                switch (userchoice)
                {
                    case 1:
                        UploadHandler.ListAlbums();
                        break;
                    case 2:
                        UploadHandler.ProcessAlbumDirectory();
                        break;
                    case 3:
                        UploadHandler.ProcessMainDirectory();
                        break;
                    default:
                        appexit = true;
                        break;
                }

            }


            //Closing out
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Exiting RR Google Photos Upload. Have a nice day!");
            Console.WriteLine();
            Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
            Console.WriteLine();
            Console.WriteLine();
        }

        private static short GetUserChoice()
        {
            char key = Char.MinValue;
            short choice = 0;

            try
            {
                key = Console.ReadKey().KeyChar;
                choice = (short)Char.GetNumericValue(key);                
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, $"Unable to convert user key to short. The user selected key '{key}' is not valid.", key);
            }

            return choice;
        }

        private static IServiceProvider BuildDi()
        {
            var services = new ServiceCollection()
                .AddLogging();

            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));

            var servicesProvider = services.BuildServiceProvider();

            var loggerFactory = servicesProvider.GetRequiredService<ILoggerFactory>();


            //configure NLog
            loggerFactory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
            NLog.LogManager.LoadConfiguration("nlog.config");

            return servicesProvider;
        }
    }
}
