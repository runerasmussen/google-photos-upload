namespace google_photos_upload.Services
{
    public interface IUploadService
    {
        bool Initialize();
        void ListAlbums();
        bool ProcessAlbumDirectory(string directorypath, bool? addifalbumexists);
        bool ProcessMainDirectory(string directorypath, bool? addifalbumexists);
    }
}