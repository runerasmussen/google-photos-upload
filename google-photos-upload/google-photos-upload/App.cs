using google_photos_upload.Services;
using Microsoft.Extensions.Logging;
using Mono.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace google_photos_upload
{
    public class App
    {
        private readonly ILogger<App> logger;
        private readonly IUploadService uploadService;

        public App(ILogger<App> logger, IUploadService uploadService)
        {
            this.logger = logger;
            this.uploadService = uploadService;
        }

        public void DoAction(string[] args)
        {
            short command = 0;
            bool shouldShowHelp = false;
            string directorypath = null;
            bool? addifalbumexists = null;
            List<string> extra;


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


            #region Command Parameter passing
            //https://github.com/xamarin/XamarinComponents/tree/master/XPlat/Mono.Options
            // these variables will be set when the command line is parsed

            //Log raw program arg values
            logger.LogDebug($"Command parameters: {String.Join("|", args)}");

            // Program argument options
            var options = new OptionSet {
                { "c=|command=", "Select Upload Command (-1 - Authentication only, 0 - User is asked, 1 - List current Google Photos Album, " +
                    "2 - Upload Single Folder into Google Photos as an Album, " +
                    "3 - Upload Multiple Folders from a main Folder into Google Photos as Albums", (short c) => command = c },
                { "d=|directory=", "Directory path to be processed", (string v) => directorypath = v },
                { "a=|addifalbumexists=", "Add media to Google Photos album if the album already exists. Value should be 'y'", a => addifalbumexists = a == "y" },
                { "h|help", "show this message and exit", h => shouldShowHelp = h != null },
            };

            try
            {
                //Set variables from program args based on the defined options
                extra = options.Parse(args);
            }
            catch (OptionException e)
            {
                logger.LogError(e, "Unexpected error occured when evaluating command parameter.");
                Console.Write("Unexpected error occured. See log for details...");
                return;
            }

            //Log variable values taken from command param args
            logger.LogDebug("Program parameters:");
            logger.LogDebug($"command: {command}");
            logger.LogDebug($"shouldShowHelp: {shouldShowHelp}");
            logger.LogDebug($"directorypath: {directorypath}");
            logger.LogDebug($"extra: {string.Join("|", extra)}");

            #endregion Command Parameter passing


            if (shouldShowHelp)
            {
                ShowHelp(options);
                return;
            }

            Execute(command, directorypath, addifalbumexists);


            //Closing out
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Exiting Google Photos Upload. Have a nice day!");
            Console.WriteLine();
            Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
            Console.WriteLine();
            Console.WriteLine();
        }

        private void Execute(short command, string directorypath, bool? addifalbumexists)
        {
            try
            {
                bool appexit = true;

                //Authenticate
                if (!uploadService.Initialize())
                {
                    logger.LogError("Critical error occured - could not establish authentication with Google");
                    Console.WriteLine("See log for details.");
                    Console.WriteLine("Press any key to exit");
                    Console.ReadKey();
                    return;
                }

                //Do the requested command
                do
                {
                    var c = Convert.ToInt16(command);

                    if (c == 0)
                    {
                        c = GetManualUserChoice();
                        appexit = false; //After the requested action has been performed ask the user again what to do next
                    }

                    switch (c)
                    {
                        case 1:
                            uploadService.ListAlbums();
                            break;
                        case 2:
                            uploadService.ProcessAlbumDirectory(directorypath, addifalbumexists);
                            break;
                        case 3:
                            uploadService.ProcessMainDirectory(directorypath, addifalbumexists);
                            break;
                        default:
                            appexit = true;
                            break;
                    }
                }
                while (!appexit);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An exception occured, program is being terminated");
            }
        }

        private static void ShowHelp(OptionSet p)
        {
            //Specify command pr. operating system
            Console.WriteLine("Usage: dotnet google-photos-upload.dll [OPTIONS]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }

        private short GetManualUserChoice()
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

            return userchoice;
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
