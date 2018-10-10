using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Google.Apis.PhotosLibrary.v1;
using google_photos_upload.Model;

namespace google_photos_upload
{
    public static class UploadHandler
    {
        private static PhotosLibraryService service = null;

        public static void Initialize()
        {
            service = ServiceHandler.GetPhotosLibraryService();

            if (service is null)
                throw new Exception("Initialize of Google Photos API Authentication failed");
        }

        public static void ListAlbums()
        {
            MyAlbum.ListAlbums(service);
        }

        public static bool ProcessMainDirectory()
        {
            Console.WriteLine("# Upload Child Folders in main Folder as Albums into Google Photos");
            Console.WriteLine("What is the path to the main Folder?");
            string path = Console.ReadLine();

            if (!Directory.Exists(path))
            {
                Console.WriteLine("The folder could not be found. Please try again.");
                return false;
            }

            DirectoryInfo mainDirInfo = new DirectoryInfo(path);
            foreach (var imgFolder in mainDirInfo.EnumerateDirectories())
            {
                bool albumuploadresult = ProcessAlbumDirectory(imgFolder.FullName);

                if (!albumuploadresult)
                {
                    Console.WriteLine($"Upload failed of Album '{imgFolder.Name}'");
                    //return false; Continue
                }
            }

            return true;
        }

        public static bool ProcessAlbumDirectory()
        {
            Console.WriteLine("# Upload Folder as Album into Google Photos");
            Console.WriteLine("What folder do you want to upload?");
            string path = Console.ReadLine();

            if (!Directory.Exists(path))
            {
                Console.WriteLine("The folder could not be found. Please try again.");
                return false;
            }

            return ProcessAlbumDirectory(path);
        }

        private static bool ProcessAlbumDirectory(string path)
        {
            if (!Directory.Exists(path))
                throw new ArgumentException($"The path '{path}' was not found.");

            DirectoryInfo dirInfo = new DirectoryInfo(path);
            string albumtitle = dirInfo.Name;

            Console.WriteLine();
            Console.WriteLine($"Uploading Album: {albumtitle}");

            MyAlbum album = new MyAlbum(service, albumtitle, dirInfo);
            bool albumuploadresult = album.UploadAlbum();

            if (!albumuploadresult)
            {
                Console.WriteLine("WARNING: One or more issues occured during the Upload. Please check the log.");
                return false;
            }

            Console.WriteLine($"Album '{albumtitle}' including {album.ImageUploadCount} images uploaded to Google Photos!");

            return true;
        }
    }
}
