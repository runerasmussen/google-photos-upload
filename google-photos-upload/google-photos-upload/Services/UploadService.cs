using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.PhotosLibrary.v1;
using google_photos_upload.Extensions;
using google_photos_upload.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace google_photos_upload.Services
{
    public class UploadService : IUploadService
    {
        private readonly ILogger<UploadService> logger;
        private readonly IAuthenticationService authenticationService;
        private PhotosLibraryService service = null;
        private DriveService driveService = null;

        public UploadService(ILogger<UploadService> logger, IAuthenticationService authenticationService)
        {
            this.logger = logger;
            this.authenticationService = authenticationService;
        }

        public bool Initialize()
        {
            service = authenticationService.GetPhotosLibraryService();
            driveService = authenticationService.GetDriveService();

            if (service is null || driveService is null)
            {
                logger.LogCritical("Initialize of Google Photos API Authentication failed");
                return false;
            }

            return true;
        }

        public void ListAlbums()
        {
            MyAlbum.ListAlbums(service, logger);
        }

        public bool ProcessMainDirectory(string directorypath, bool? addifalbumexists)
        {
            var albumUploadResults = new List<Tuple<bool, string>>();
            string path = directorypath;

            if (path is null)
            {
                Console.WriteLine("# Upload Child Folders in main Folder as Albums into Google Photos");
                Console.WriteLine("What is the path to the main Folder?");
                path = Console.ReadLine();
            }

            path = path.RemoveOsPathEscapeCharacters();

            if (!Directory.Exists(path))
            {
                Console.WriteLine($"The folder '{path}' could not be found. Please try again.");
                return false;
            }

            DirectoryInfo mainDirInfo = new DirectoryInfo(path);
            foreach (var imgFolder in mainDirInfo.GetDirectories().OrderBy(di => di.Name))
            {
                var albumuploadresult = ProcessAlbumDirectoryUpload(imgFolder.FullName, addifalbumexists);

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

        public bool ProcessAlbumDirectory(string directorypath, bool? addifalbumexists)
        {
            string path = directorypath;

            //If directory path is not provided from command parameter then ask user what the path is
            if (path is null)
            {
                Console.WriteLine("# Upload Folder as Album into Google Photos");
                Console.WriteLine("What folder do you want to upload?");
                path = Console.ReadLine();
            }

            path = path.RemoveOsPathEscapeCharacters();

            if (!Directory.Exists(path))
            {
                logger.LogError($"The file path could not be found: '{path}'");
                Console.WriteLine("The folder could not be found. Please try again.");
                return false;
            }


            //Process album
            var uploadResult = ProcessAlbumDirectoryUpload(path, addifalbumexists);


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

        private bool SpaceAvailableForMediaFolder(DirectoryInfo directoryInfo)
        {
            long bufferspace = 0;
            var foldersize = directoryInfo.GetDirectorySize();
            var googleDriveSpaceAvailable = GetGoogleDriveSpaceAvailable();

            if ((googleDriveSpaceAvailable - bufferspace) < foldersize)
            {
                var foldersizeMb = foldersize / 1024.0F / 1024.0F;
                var googleDriveSpaceAvailableMb = googleDriveSpaceAvailable / 1024.0F / 1024.0F;

                logger.LogWarning("There is not sufficient space available in your Google Account to upload this folder.");
                logger.LogWarning($"{foldersizeMb} Mb required, only {googleDriveSpaceAvailableMb} available");

                return false;
            }

            return true;
        }


        private long GetGoogleDriveSpaceAvailable()
        {
            long storageAvailable = -1;

            AboutResource.GetRequest getRequest = driveService.About.Get();
            getRequest.Fields = "*";
            About about = getRequest.Execute();

            long? storageLimit = about.StorageQuota.Limit;
            long? storageUsage = about.StorageQuota.Usage;



            if (!(storageLimit is null || storageUsage is null))
            {
                long storageLimitValue = (long)storageLimit;
                long storageUsageValue = (long)storageUsage;

                storageAvailable = storageLimitValue - storageUsageValue;
            }

            logger.LogDebug($"Google account space limit (bytes): {storageLimit}");
            logger.LogDebug($"Google account space used (bytes): {storageUsage}");
            logger.LogDebug($"Google account space available (bytes): {storageAvailable}");

            return storageAvailable;
        }

        private (bool uploadResult, string uploadResultText) ProcessAlbumDirectoryUpload(string path, bool? addifalbumexists)
        {
            try
            {
                if (!Directory.Exists(path))
                    throw new ArgumentException($"The path '{path}' was not found.");

                DirectoryInfo dirInfo = new DirectoryInfo(path);


                //Verify there is sufficient disk space
                if (!SpaceAvailableForMediaFolder(dirInfo))
                    return (false, "Album not uploaded. Not sufficient storage space in Google Photos.");


                
                string albumtitle = dirInfo.Name;

                Console.WriteLine();
                Console.WriteLine($"Uploading Album: {albumtitle}");

                MyAlbum album = new MyAlbum(logger, service, albumtitle, dirInfo);


                //Does the album already exist?
                if (!album.IsAlbumNew)
                {
                    if (!album.IsAlbumWritable)
                    {
                        return (false, "Album not updated. For safety reasons then an album created outside this utility is not updated.");
                    }
                    else
                    {
                        //Ask user if existing Album should be updated, if answer not provided through program args
                        if (addifalbumexists == null)
                        {
                            Console.Write("The album already exists, do you want to add any missing images to it? (y/n) ");

                            try
                            {
                                char key = Console.ReadKey().KeyChar;

                                if (key != 'y')
                                {
                                    Console.WriteLine();
                                    album.UploadStatus = UploadStatus.UploadAborted;
                                    return (false, album.ToStringUploadResult());
                                }
                            }
                            catch (Exception e)
                            {
                                logger.LogError(e, "An error occured when evaluating user input");
                                Console.WriteLine();
                                return (false, "An unexpected error occured, check the log");
                            }

                            Console.WriteLine();
                        }
                        else if (addifalbumexists == false)
                        {
                            logger.LogInformation("The album already exists and is not updated.");
                            return (false, album.ToStringUploadResult());
                        }
                    }
                }


                //Upload the album and images to Google Photos
                album.UploadAlbum();


                //Upload complete, share the result
                return (true, album.ToStringUploadResult());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"An exception occured during processing of '{path}'");

                return (false, $"{path}: An exception occured during Album upload");
            }
        }

    }
}
