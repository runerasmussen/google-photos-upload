using Google.Apis.Auth.OAuth2;
using Google.Apis.PhotosLibrary.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace google_photos_upload.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/rr-google-photos-upload.json
        static readonly string[] Scopes = {
            PhotosLibraryService.Scope.PhotoslibraryAppendonly,
            PhotosLibraryService.Scope.PhotoslibraryReadonlyAppcreateddata,
            PhotosLibraryService.Scope.PhotoslibraryReadonly
        };
        private const string ApplicationName = "RR Google Photos Upload";
        private readonly ILogger<AuthenticationService> logger;
        private UserCredential credential = null;


        //Constructor
        public AuthenticationService(ILogger<AuthenticationService> logger)
        {
            this.logger = logger;
        }


        /// <summary>
        /// Perform an authentication and authorization via the default web browser
        /// to enable authorization for the App to access/edit Google Photos content.
        /// </summary>
        private void AuthenticateAuthorize()
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
                    logger.LogInformation("The application requires your permission to access to Google Photos account.");
                    logger.LogInformation("Your default web browser will open now with a Google Authentication webpage");
                    logger.LogInformation("so you safely can grant permission to this app.");
                    Thread.Sleep(5000);
                }

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;

                if (credential == null)
                    throw new System.Security.Authentication.AuthenticationException("Could not authenticate");

                if (newlyAuthenticated)
                {
                    logger.LogInformation($"Credential file saved to: {credPath}");
                }

                logger.LogInformation("Authentiation complete.");
            }
        }

        /// <summary>
        /// Get an instance of the Google Photos Library Service (includes authentication and authorization)
        /// </summary>
        /// <returns></returns>
        public PhotosLibraryService GetPhotosLibraryService()
        {
            if (credential == null)
                AuthenticateAuthorize();

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
