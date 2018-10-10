using Google.Apis.PhotosLibrary.v1;
using Google.Apis.PhotosLibrary.v1.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using ExifLibrary;
using google_photos_upload.Extensions;

namespace google_photos_upload.Model
{
    class MyImage
    {
        private PhotosLibraryService service = null;
        private Album album = null;
        private FileInfo imgFile = null;
        private string uploadToken = null;

        private static readonly string[] allowedfileformats = { "jpg", "jpeg", "gif" };

        public MyImage(PhotosLibraryService photoService, Album album, FileInfo imgFile)
        {
            this.service = photoService;
            this.album = album;
            this.imgFile = imgFile;
        }

        public string Name
        {
            get { return imgFile.Name; }
        }

        private string NameEncoded
        {
            get { return Name.UrlEncode(); }
        }

        private string NameASCII
        {
            get { return Name.UnicodeToASCII(); }
        }

        public string UploadToken
        {
            get { return this.uploadToken; }
        }


        /// <summary>
        /// 'Date Taken Original' value from image EXIF data. Avoids loading the whole image file into memory.
        /// </summary>
        /// <param name="path">Filepath to image</param>
        /// <returns>'Date Taken Original' value from EXIF converted into DateTime format</returns>
        private static DateTime? GetDateImageWasTaken(string path)
        {
            try
            {
                ImageFile imageFile = ImageFile.FromFile(path);
                var datetimeOriginal = imageFile.Properties.FirstOrDefault<ExifProperty>(x => x.Tag == ExifTag.DateTimeOriginal).Value;
                string datetimeOriginaltxt = datetimeOriginal.ToString();

                if (DateTime.TryParse(datetimeOriginaltxt, out DateTime dtOriginal))
                    return dtOriginal;

                //Unable to get date
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed reading EXIF data...");
            }

            //Unable to extract EXIF data from file
            return null;
        }


        public bool IsFormatSupported
        {
            get {
                try
                {
                    //Verify file extension is supported.
                    string filename = imgFile.Name.ToLower();
                    string fileext = Path.GetExtension(filename);
                    if (!allowedfileformats.Any(fileext.Contains))
                        return false;


                    // EXIF property "Date Taken Original" must be set on the image file to ensure correct date in Google Photos
                    var datetimeOriginal = GetDateImageWasTaken(imgFile.FullName);
                    if (datetimeOriginal == null)
                        return false;
                }
                catch (Exception)
                {
                    //What to do?
                    return false;
                }

                return true;
            }
        }


        public bool UploadImage()
        {
            //Upload Photo to Google Photo backend
            this.uploadToken = UploadPhotoFile(service);

            if (uploadToken is null)
                return false;

            return true;
        }


        /// <summary>
        /// Upload photo to Google Photos.
        /// API guide for media upload:
        /// https://developers.google.com/photos/library/guides/upload-media
        /// 
        /// The implementation is inspired by this post due to limitations in the Google Photos API missing a method for content upload,
        /// but instead of System.Net.HttpClient it has been rewritten to use Google Photo's HTTP Client as it the  has the Bearer Token already handled.
        /// https://stackoverflow.com/questions/51576778/upload-photos-to-google-photos-api-error-500
        /// </summary>
        /// <param name="photo"></param>
        /// <returns></returns>
        private string UploadPhotoFile(PhotosLibraryService photoService)
        {
            string uploadToken = null;

            try
            {
                using (var fileStream = imgFile.OpenRead())
                {
                    //Create byte array to store the image for transfer
                    byte[] pixels = new byte[fileStream.Length];

                    //Read image into the pixels byte array
                    fileStream.Read(pixels, 0, (int)fileStream.Length);

                    var httpContent = new ByteArrayContent(pixels);
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    httpContent.Headers.Add("X-Goog-Upload-File-Name", NameASCII);
                    httpContent.Headers.Add("X-Goog-Upload-Protocol", "raw");


                    HttpResponseMessage photoResponse = photoService.HttpClient.PostAsync("https://photoslibrary.googleapis.com/v1/uploads", httpContent).Result;

                    if (photoResponse.IsSuccessStatusCode)
                        uploadToken = photoResponse.Content.ReadAsStringAsync().Result;
                }

            }
            catch (Exception)
            {
                throw;
            }

            photoService.HttpClient.DefaultRequestHeaders.Clear();

            return uploadToken;
        }

    }
}
