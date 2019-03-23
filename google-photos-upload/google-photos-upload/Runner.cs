using google_photos_upload.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace google_photos_upload
{
    public class Runner
    {
        private readonly ILogger<Runner> logger;
        private readonly UploadService uploadService;

        public Runner(ILogger<Runner> logger, UploadService uploadService)
        {
            this.logger = logger;
            this.uploadService = uploadService;
        }

        public void DoAction()
        {
            DrawConsoleInterface();
        }

        private void DrawConsoleInterface()
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

            if (!uploadService.Initialize())
            {
                Console.WriteLine("Critical error occured - could not establish authentication with Google");
                Console.WriteLine("See log for details.");
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();

                appexit = true;
            }


            try
            {
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
                            uploadService.ListAlbums();
                            break;
                        case 2:
                            uploadService.ProcessAlbumDirectory();
                            break;
                        case 3:
                            uploadService.ProcessMainDirectory();
                            break;
                        default:
                            appexit = true;
                            break;
                    }

                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An exception occured, program is being terminated");
            }


            //Closing out
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Exiting Google Photos Upload. Have a nice day!");
            Console.WriteLine();
            Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
            Console.WriteLine();
            Console.WriteLine();
        }

        private short GetUserChoice()
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
                logger.LogWarning(e, $"Unable to convert user key to short. The user selected key '{key}' is not valid.", key);
            }

            return choice;
        }
    }
}
