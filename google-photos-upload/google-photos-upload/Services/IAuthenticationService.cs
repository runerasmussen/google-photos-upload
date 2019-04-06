using Google.Apis.PhotosLibrary.v1;
using Google.Apis.Drive.v3;

namespace google_photos_upload.Services
{
    public interface IAuthenticationService
    {
        PhotosLibraryService GetPhotosLibraryService();

        DriveService GetDriveService();
    }
}