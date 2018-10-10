using System;
using System.IO;
using System.Threading;
using Google.Apis.PhotosLibrary.v1;
using System.Collections.Generic;

namespace google_photos_upload
{
    class Program
    {
        static void Main(string[] args)
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

            UploadHandler.Initialize();

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
        }


    }
}
