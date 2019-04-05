using Google.Apis.PhotosLibrary.v1;

namespace google_photos_upload.Services
{
    public interface IAuthenticationService
    {
        PhotosLibraryService GetPhotosLibraryService();
    }
}