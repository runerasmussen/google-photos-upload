using Google.Apis.PhotosLibrary.v1;
using Google.Apis.PhotosLibrary.v1.Data;
using System;
using System.Collections.Generic;
using System.IO;
using google_photos_upload.Extensions;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace google_photos_upload.Model
{
    class MyAlbum
    {
        private readonly ILogger _logger;

        private string albumTitle = null;
        private Album album = null;
        private readonly PhotosLibraryService service = null;
        private readonly DirectoryInfo dirInfo = null;
        private List<MyImage> myImages = new List<MyImage>();

        public MyAlbum(ILogger logger, PhotosLibraryService service, string albumTitle, DirectoryInfo dirInfo)
        {
            this._logger = logger;
            this.service = service;
            this.albumTitle = albumTitle;
            this.dirInfo = dirInfo;
        }

        /// <summary>
        /// Number of photos
        /// </summary>
        public int ImageUploadCount
        {
            get { return myImages.Count; }
        }


        /// <summary>
        /// Verify this is a new Album that does not exist in Google Photos
        /// </summary>
        public bool IsAlbumNew
        {
            get
            {
                SetAlbum();
                return album == null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsAlbumWritable
        {
            get
            {
                SetAlbum(); //Consider a way to avoid this check to save API calls.
                if (album == null)
                    return false;

                return album.IsWriteable.GetValueOrDefault(false);
            }
        }


        public static void ListAlbums(PhotosLibraryService service, ILogger logger)
        {
            logger.LogInformation("");
            logger.LogInformation("Fetching albums...");

            Google.Apis.PhotosLibrary.v1.AlbumsResource.ListRequest request = service.Albums.List();

            // List events.
            Google.Apis.PhotosLibrary.v1.Data.ListAlbumsResponse response = request.Execute();
            logger.LogInformation("Albums:");
            if (response.Albums != null && response.Albums.Count > 0)
            {
                while (response.Albums != null && response.Albums.Count > 0)
                {
                    foreach (var albumresponse in response.Albums)
                    {
                        string title = albumresponse.Title;
                        logger.LogInformation($"> {title}");
                    }

                    if (response.NextPageToken != null)
                    {
                        request.PageToken = response.NextPageToken;
                        response = request.Execute();
                    }
                    else
                        break;
                }
            }
            else
            {
                logger.LogInformation("No albums found.");
            }
        }



        /// <summary>
        /// Process a directory by first uploading all images to Google Cloud and secondly adding them to an Album.
        /// If the Album exists the images will be added to it.
        /// </summary>
        public bool UploadAlbum()
        {
            //Check if Album already exists and if it's writable
            //Get Album if it exists
            if (!IsAlbumNew &&!IsAlbumWritable)
            {
                _logger.LogError($"Album '{albumTitle}' already exists and is not writable (was created outside of this utility). For safety reasons by design such Albums will not be updated.");
                return false;
            }


            //Upload images to Google Cloud
            if (!UploadImages())
            {
                _logger.LogError($"Album '{albumTitle}' image file(s) upload failed fully/partly");
            }

            //Abort if zero images uploaded
            if (ImageUploadCount == 0 || myImages.Count == 0)
            {
                _logger.LogError("Zero images were succesfully uploaded. Album will not be created");
                return false;
            }


            if (album is null)
            {
                //New Album
                _logger.LogInformation($"Creating new Album in Google Photos: {albumTitle}");

                //Create Album in Google Photos
                album = CreateAlbum();

                //Get Album that was just created.
                //This is needed due to a bug where Google Photos API does not set the IsWritable when it's created but only when fetched again.
                if (album != null && album.IsWriteable is null)
                {
                    album = SetAlbum();
                }

                if (album is null)
                {
                    _logger.LogError($"Album '{albumTitle}' not found after it was created. Aborting.");
                    return false;
                }

                if (!albumTitle.Equals(album.Title))
                {
                    //Google Photos do not support special characters well, 
                    //use the Title that it was ended up being in Google Photos.
                    albumTitle = album.Title;
                }
            }
            else
            {
                //Album already exists, update it
                _logger.LogWarning($"Album '{albumTitle}' already exists in Google Photos and will be updated");
            }


            //Add the uploaded images to the Users Google Photo account and into a specific album
            if (!AddPhotosToAlbum())
                return false;

            return true;
        }


        private bool UploadImages()
        {
            bool uploadresult = true;

            foreach (var imgFile in dirInfo.GetFiles().OrderBy(fi => fi.Name))
            {
                if (!imgFile.Attributes.HasFlag(FileAttributes.Hidden))  //Do not process hidden files
                {
                    MyImage myImage = new MyImage(_logger, service, imgFile);
                    _logger.LogInformation($"Uploading {myImage.Name}");

                    if (myImage.IsFormatSupported)
                    {

                        bool imguploadresult = myImage.UploadImage();

                        if (!imguploadresult)
                        {
                            uploadresult = false;
                            _logger.LogError("Image upload failed");
                        }
                        else
                        {
                            myImages.Add(myImage);
                        }
                    }
                    else
                    {
                        uploadresult = false;
                        _logger.LogWarning($"NOT uploading '{myImage.Name}' due to file type not supported or EXIF data issue");
                    }
                }
            }

            return uploadresult;
        }


        /// <summary>
        /// Get the existing Google Photos Album entry.
        /// Sets the class variable 'album' as well.
        /// </summary>
        /// <returns>Album that exists, null otherwise</returns>
        private Album SetAlbum()
        {
            if (album != null)
            {
                return album;
            }

            AlbumsResource.ListRequest request = service.Albums.List();
            //request.PageSize = 10; //Uncommenting. Let Google decide the page size.
            ListAlbumsResponse response = request.Execute();

            string alternateAlbumTitle = albumTitle.Replace("&", "&amp;");

            while (response.Albums != null && response.Albums.Count > 0)
            {
                foreach (var albumresponse in response.Albums)
                {
                    if (albumresponse.Title.Equals(albumTitle) || albumresponse.Title.Equals(alternateAlbumTitle))
                    {
                        album = albumresponse;
                        return albumresponse;
                    }
                }

                //Fetch next page of Albums
                if (response.NextPageToken != null)
                {
                    request.PageToken = response.NextPageToken;
                    response = request.Execute();
                }
                else
                    break;
            }

            return null;
        }


        private Album CreateAlbum()
        {
            Album albumNew = new Album();
            albumNew.Title = albumTitle;

            CreateAlbumRequest createAlbumRequest = new CreateAlbumRequest();
            createAlbumRequest.Album = albumNew;

            Album responseAlbum = service.Albums.Create(createAlbumRequest).Execute();

            _logger.LogInformation($"Album created: {responseAlbum.Title}");


            return responseAlbum;
        }


        private bool AddPhotosToAlbum()
        {
            if (album == null)
                throw new ArgumentException("Album is NULL - image upload aborted");
            if (album.IsWriteable == null || !album.IsWriteable.Value)
                throw new ArgumentException("Album is not writable - image upload aborted");

            _logger.LogInformation($"Adding {myImages.Count} images to Album '{albumTitle}'");

            const int maxBatchSize = 49;
            int imagesAddedToAlbum = 0;

            //Divide into batches, due to API limitation
            var batches = myImages.Batch<MyImage>(maxBatchSize);

            foreach (var batch in batches)
            {
                imagesAddedToAlbum += ImageToAlbumBatch(batch);
                _logger.LogDebug($"Added {imagesAddedToAlbum} images to Album '{albumTitle}'");
            }


            if (myImages.Count != imagesAddedToAlbum)
            {
                _logger.LogError($"Images not added fully to Album. Expected {myImages.Count}, only {imagesAddedToAlbum} added.");
                return false;
            }

            return true;
        }

        private int ImageToAlbumBatch(IEnumerable<MyImage> batch)
        {
            var imagecollection = new List<NewMediaItem>();

            foreach (var myImage in batch)
            {
                _logger.LogDebug($"Adding '{myImage.Name}' to album");

                NewMediaItem image = new NewMediaItem();
                image.Description = Path.GetFileNameWithoutExtension(myImage.Name);
                image.SimpleMediaItem = new SimpleMediaItem()
                {
                    UploadToken = myImage.UploadToken
                };

                imagecollection.Add(image);
            }


            BatchCreateMediaItemsRequest batchCreateMediaItemsRequest = new BatchCreateMediaItemsRequest
            {
                AlbumId = album.Id,
                NewMediaItems = imagecollection
            };

            MediaItemsResource.BatchCreateRequest batchCreateRequest = service.MediaItems.BatchCreate(batchCreateMediaItemsRequest);

            BatchCreateMediaItemsResponse batchCreateMediaItemsResponse = batchCreateRequest.Execute();
            return batchCreateMediaItemsResponse.NewMediaItemResults.Count;
        }
    }
}
