using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Google.Apis.PhotosLibrary.v1;
using google_photos_upload.Model;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace google_photos_upload
{
    public static class UploadHandler
    {
        private static ILogger _logger;

        private static PhotosLibraryService service = null;

        public static bool Initialize(ILogger logger)
        {
            _logger = logger;

            service = ServiceHandler.GetPhotosLibraryService();

            if (service is null)
            {
                _logger.LogCritical("Initialize of Google Photos API Authentication failed");
                return false;
            }

            return true;
        }

        public static void ListAlbums()
        {
            MyAlbum.ListAlbums(service, _logger);
        }

        public static bool ProcessMainDirectory()
        {
            var albumUploadResults = new List<Tuple<bool, string>>();

            Console.WriteLine("# Upload Child Folders in main Folder as Albums into Google Photos");
            Console.WriteLine("What is the path to the main Folder?");
            string path = Console.ReadLine();

            if (!Directory.Exists(path))
            {
                Console.WriteLine($"The folder '{path}' could not be found. Please try again.");
                return false;
            }

            DirectoryInfo mainDirInfo = new DirectoryInfo(path);
            foreach (var imgFolder in mainDirInfo.GetDirectories().OrderBy(di => di.Name))
            {
                var albumuploadresult = ProcessAlbumDirectory(imgFolder.FullName);

                albumUploadResults.Add(new Tuple<bool, string>(albumuploadresult.uploadResult, albumuploadresult.uploadResultText));

                if (!albumuploadresult.uploadResult)
                {
                    Console.WriteLine($"Upload failed of Album '{imgFolder.Name}'");
                }
            }

            //Print summary for user
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("------------------------");
            Console.WriteLine("Upload summary:");

            albumUploadResults.ForEach(x => Console.WriteLine(x.Item2));

            Console.WriteLine();
            Console.WriteLine();

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


            //Process album
            var uploadResult = ProcessAlbumDirectory(path);


            //Print summary for user
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("------------------------");
            Console.WriteLine("Upload summary:");

            Console.WriteLine(uploadResult.uploadResultText);

            Console.WriteLine();
            Console.WriteLine();

            return uploadResult.uploadResult;
        }

        private static (bool uploadResult, string uploadResultText) ProcessAlbumDirectory(string path)
        {
            if (!Directory.Exists(path))
                throw new ArgumentException($"The path '{path}' was not found.");

            DirectoryInfo dirInfo = new DirectoryInfo(path);
            string albumtitle = dirInfo.Name;

            Console.WriteLine();
            Console.WriteLine($"Uploading Album: {albumtitle}");

            MyAlbum album = new MyAlbum(_logger, service, albumtitle, dirInfo);


            //Does the album already exist?
            if (!album.IsAlbumNew)
            {
                if (!album.IsAlbumWritable)
                {
                    return (false, "Album not updated. For safety reasons then album created outside this utility is not updated.");
                }
                else
                {
                    Console.Write("The album already exists, do you want to add any missing images to it? (y/n) ");

                    try
                    {
                        char key = Console.ReadKey().KeyChar;

                        if (key != 'y')
                        {
                            Console.WriteLine();
                            album.UploadStatus = UploadStatusEnum.UploadAborted;
                            return (false, album.ToStringUploadResult());
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "An error occured when evaluating user input");
                        Console.WriteLine();
                        return (false, "An unexpected error occured, check the log");
                    }

                    Console.WriteLine();
                }
            }


            //Upload the album and images to Google Photos
            bool albumuploadresult = album.UploadAlbum();


            //Upload complete, share the result
            return (true, album.ToStringUploadResult());
        }
    }
}
