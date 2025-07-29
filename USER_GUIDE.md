# Google Photos Upload - User Guide
Read about the Google Photos Upload project [here](README.md).

Ready to give it a try? Please note that this is a hobby project and comes without any warranty, but seem to work just fine!

If you run into an issue then take a look at the log file found locally on your computer in log/google-photos-upload.log


## Step by Step - Windows
1. Download the latest release for Windows [here](https://github.com/runerasmussen/google-photos-upload/releases/latest).
2. Run google-photos-upload.exe (or run in a terminal using [commandline parameters](#Commandline-Parameters)). It will ask for [permission](docs/google-photos-permissions.md) on the first run.
3. Follow the instructions in the application to upload images/videos.


## Step by Step - Mac/Linux
*dotnet runtime installation required.*
1. Install [.NET](https://www.microsoft.com/net/download)
2. Download the latest cross-platform release [here](https://github.com/runerasmussen/google-photos-upload/releases/latest).
3. Open a terminal and run this command: dotnet google-photos-upload.dll 
   (or run in a terminal using [commandline parameters](#Commandline-Parameters)). It will ask for [permission](docs/google-photos-permissions.md) on the first run.
4. Follow the instructions in the application to upload images/videos.


## Things to note
 * Configuration options in the file `google-photos-upload.dll.config`:
   * `<add key="IMG_UPLOAD_NO_EXIF" value="true"/>`: Set this value to `true` to prevent image upload if EXIF data is missing.

 * The Google Photos Media title will be derived from the first available of these:
   1. EXIF ImageDescription
   2. Filename without file extension

 * Supported file types: mov, avi, jpg, jpeg, gif
 
 * Ignored file types (other file types will give a warning message): txt, thm

 * Media will be uploaded in uncompressed 'Original Quality'. The Google Photos API does not support uploading in 'High Quality'. 
   To recover storage; Open [Google Photos Settings website](https://photos.google.com/settings) and 
   Click 'Recover Storage' (will downgrade **ALL** your photos in Google Photos from Original down to High Quality).


## Commandline Parameters
 * Windows: in a terminal execute "dotnet google-photos-upload.exe -h"
 * Mac/Linux: in a terminal execute "dotnet google-photos-upload.dll -h"

Available parameters:

Parameter | Comment
--------- | -----------
-c=<br />-command= | Select Upload Command:<br/>-1 - Authentication only<br/>0 - User is asked<br/>1 - List current Google Photos Album<br/>2 - Upload Single Folder into Google Photos as an Album<br/>3 - Upload Multiple Folders from a main Folder into Google Photos as Albums
-d=<br/>-directory= | Directory path to be processed
-a=<br/>-addifalbumexists= | Add media to Google Photos album if the album already exists. Value should be 'y'
-h<br/>-help | Show parameter options and exit


## Optional: Obtain a Google Photos API key
The Google Photos API key is required to allow upload.
There is a key provided in the source code which has constraints, but sufficient for testing.
1. Obtain a Google Photos API key (Client ID and Client Secret) by following the instructions on [Getting started with Google Photos REST APIs](https://developers.google.com/photos/library/guides/get-started)

    * **NOTE** When selecting your application type in Step 4 of "Request an OAuth 2.0 client ID", please select "Other". There's also no need to carry out step 5 in that section.

2. Replace `YOUR_CLIENT_ID` in the client_id.json file with the provided Client ID. 
3. Replace `YOUR_CLIENT_SECRET` in the client_id.json file wiht the provided Client Secret.


[![analytics](https://www.google-analytics.com/collect?v=1&t=pageview&tid=UA-3234978-2&cid=4baccbc6-a605-4558-9dd4-ccb8899aa950&dp=%2FUSER_GUIDE.md&dh=github.com
)]()