﻿using System;
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


            UploadHandler.Initialize(_logger);

            bool appexit = false;

            do
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

                char key = Console.ReadKey().KeyChar;
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("--------------------------------------------------------------------------------");

                switch (key)
                {
                    case '1':
                        UploadHandler.ListAlbums();
                        break;
                    case '2':
                        UploadHandler.ProcessAlbumDirectory();
                        break;
                    case '3':
                        UploadHandler.ProcessMainDirectory();
                        break;
                    default:
                        appexit = true;
                        break;
                }

            } while (!appexit);


            //Closing out
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Exiting RR Google Photos Upload. Have a nice day!");
            Console.WriteLine();
            Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
            Console.WriteLine();
            Console.WriteLine();

            // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
            NLog.LogManager.Shutdown();
        }


        private static IServiceProvider BuildDi()
        {
            var services = new ServiceCollection()
                .AddLogging();

            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            //services.AddLogging((builder) => builder.SetMinimumLevel(LogLevel.Trace));

            var servicesProvider = services.BuildServiceProvider();

            var loggerFactory = servicesProvider.GetRequiredService<ILoggerFactory>();


            //configure NLog
            loggerFactory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
            NLog.LogManager.LoadConfiguration("nlog.config");

            return servicesProvider;
        }
    }
}