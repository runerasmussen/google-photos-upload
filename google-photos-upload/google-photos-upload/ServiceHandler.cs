using Google.Apis.Auth.OAuth2;
using Google.Apis.PhotosLibrary.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.IO;
using System.Threading;

namespace google_photos_upload
{
    static class ServiceHandler
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/rr-google-photos-upload.json
        static readonly string[] Scopes = {
            PhotosLibraryService.Scope.PhotoslibraryAppendonly,
            PhotosLibraryService.Scope.PhotoslibraryReadonlyAppcreateddata,
            PhotosLibraryService.Scope.PhotoslibraryReadonly
        };
        static readonly string ApplicationName = "RR Google Photos Upload";

        static UserCredential credential = null;


        private static void Authenticate()
        {
            using (var stream =
                new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/rr-google-photos-upload.json");
                bool newlyAuthenticated = false;

                if (!Directory.Exists(credPath) || Directory.GetFiles(credPath).Length == 0)
                {
                    newlyAuthenticated = true;
                    Console.WriteLine("The application requires your permission to access to Google Photos account.");
                    Console.WriteLine("Your default web browser will open now with a Google Authentication webpage");
                    Console.WriteLine("so you safely can grant permission to this app.");
                    Thread.Sleep(5000);
                }

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;

                if (credential == null)
                    throw new Exception("Could not authenticate");

                if (newlyAuthenticated)
                {
                    Console.WriteLine($"Credential file saved to: {credPath}");
                }

                Console.WriteLine("Authentiation complete.");
            }
        }

        public static PhotosLibraryService GetPhotosLibraryService()
        {
            if (credential == null)
                Authenticate();

            // Create Google Photos API service.
            var service = new PhotosLibraryService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
            return service;
        }
    }
}
