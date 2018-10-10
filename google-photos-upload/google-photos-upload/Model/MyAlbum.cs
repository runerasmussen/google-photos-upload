using Google.Apis.PhotosLibrary.v1;
using Google.Apis.PhotosLibrary.v1.Data;
using System;
using System.Collections.Generic;
using System.IO;
using google_photos_upload.Extensions;

namespace google_photos_upload.Model
{
    class MyAlbum
    {
        private string albumTitle = null;
        private Album album = null;
        private PhotosLibraryService service = null;
        private DirectoryInfo dirInfo = null;
        private List<MyImage> myImages = new List<MyImage>();

        public MyAlbum(PhotosLibraryService service, string albumTitle, DirectoryInfo dirInfo)
        {
            this.service = service;
            this.albumTitle = albumTitle;
            this.dirInfo = dirInfo;
        }

        public int ImageUploadCount
        {
            get { return myImages.Count; }
        }

        private string AlbumTitleGoogleFriendly
        {
            get { return albumTitle.ToUTF8(); }
        }


        public static void ListAlbums(PhotosLibraryService service)
        {
            Console.WriteLine();
            Console.WriteLine("Fetching albums...");

            Google.Apis.PhotosLibrary.v1.AlbumsResource.ListRequest request = service.Albums.List();

            // List events.
            Google.Apis.PhotosLibrary.v1.Data.ListAlbumsResponse response = request.Execute();
            Console.WriteLine("Albums:");
            if (response.Albums != null && response.Albums.Count > 0)
            {
                bool morePages = true;

                while (response.Albums != null && response.Albums.Count > 0 && morePages)
                {
                    foreach (var album in response.Albums)
                    {
                        string title = album.Title;
                        Console.WriteLine($"> {title}");
                    }

                    if (response.NextPageToken != null)
                    {
                        request.PageToken = response.NextPageToken;
                        response = request.Execute();
                    }
                    else
                    {
                        morePages = false;
                    }
                }
            }
            else
            {
                Console.WriteLine("No albums found.");
            }
        }


        public bool UploadAlbum()
        {
            //Upload images to Google Cloud
            if (!UploadImages())
            {
                Console.WriteLine($"Album '{albumTitle}' image file(s) upload failed fully/partly");
            }

            //Abort if zero images uploaded
            if (ImageUploadCount == 0 || myImages.Count == 0)
            {
                Console.WriteLine("Zero images were succesfully uploaded. Album will not be created");
                return false;
            }


            //Get Album if it exists
            album = GetAlbum();

            if (album is null)
            {
                //New Album
                Console.WriteLine($"Creating new Album in Google Photos: {albumTitle}");

                //Create Album in Google Photos
                album = CreateAlbum();

                //Get Album that was just created.
                //This is needed due to a bug where Google Photos API does not set the IsWritable when it's created but only when fetched again.
                if (album != null && album.IsWriteable is null)
                {
                    album = GetAlbum();
                }

                if (album is null)
                {
                    Console.WriteLine($"Album '{albumTitle}' not found after it was created. Aborting.");
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
                Console.WriteLine($"Album '{albumTitle}' already exists in Google Photos and will be updated");
            }


            //Add the uploaded images to the Users Google Photo account and into a specific album
            if (!AddPhotosToAlbum())
                return false;

            return true;
        }


        private bool UploadImages()
        {
            bool uploadresult = true;

            foreach (var imgFile in dirInfo.EnumerateFiles())
            {
                if (!imgFile.Attributes.HasFlag(FileAttributes.Hidden))  //Do not process hidden files
                {
                    MyImage myImage = new MyImage(service, album, imgFile);
                    Console.WriteLine($"Uploading {myImage.Name}");

                    if (myImage.IsFormatSupported)
                    {

                        bool imguploadresult = myImage.UploadImage();

                        if (!imguploadresult)
                        {
                            uploadresult = false;
                            Console.WriteLine("Image upload failed");
                        }
                        else
                        {
                            myImages.Add(myImage);
                        }
                    }
                    else
                    {
                        uploadresult = false;
                        Console.WriteLine($"NOT uploading '{myImage.Name}' due to file type not supported or EXIF data issue");
                    }
                }
            }

            return uploadresult;
        }



        private Album GetAlbum()
        {
            AlbumsResource.ListRequest request = service.Albums.List();
            ListAlbumsResponse response = request.Execute();

            if (response.Albums != null && response.Albums.Count > 0)
            {
                bool morePages = true;

                while (response.Albums != null && response.Albums.Count > 0 && morePages)
                {
                    foreach (var album in response.Albums)
                    {
                        string alternateAlbumTitle = albumTitle.Replace("&", "&amp;");

                        if (album.Title.Equals(albumTitle) || album.Title.Equals(alternateAlbumTitle))
                            return album;
                    }

                    if (response.NextPageToken != null)
                    {
                        request.PageToken = response.NextPageToken;
                        response = request.Execute();
                    }
                    else
                    {
                        morePages = false;
                    }
                }
            }

            return null;
        }


        private Album CreateAlbum()
        {
            Album album = new Album();
            album.Title = albumTitle;

            CreateAlbumRequest createAlbumRequest = new CreateAlbumRequest();
            createAlbumRequest.Album = album;

            Album responseAlbum = service.Albums.Create(createAlbumRequest).Execute();

            Console.WriteLine($"Album created: {responseAlbum.Title}");


            return responseAlbum;
        }


        private bool AddPhotosToAlbum()
        {
            if (album == null)
                throw new ArgumentException("Album is NULL - image upload aborted");
            if (album.IsWriteable == null || !album.IsWriteable.Value)
                throw new ArgumentException("Album is not writable - image upload aborted");

            Console.WriteLine($"Adding {myImages.Count} images to Album '{albumTitle}'");

            var imagecollection = new List<NewMediaItem>();

            foreach (var myImage in myImages)
            {
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
            int imagesAddedToAlbum = batchCreateMediaItemsResponse.NewMediaItemResults.Count;

            if (myImages.Count != imagesAddedToAlbum)
                throw new Exception($"Images not added to Album. Expected {myImages.Count}, only {imagesAddedToAlbum} added.");

            return true;
        }

    }
}
