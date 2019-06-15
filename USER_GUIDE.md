# Google Photos Upload - User Guide
Read about the Google Photos Upload project [here](README.md).

Ready to give it a try? Please note that this is a hobby project and comes without any warranty, but seem to work just fine!

If you run into an issue then take a look at the log file found in log/google-photos-upload.log


## Windows 10 Standalone User Guide
*No requirement for dotnet core installation.*
1. Download the latest **Win10** compatible release [here](https://github.com/runerasmussen/google-photos-upload/releases/latest)
2. Run google-photos-upload.exe (or use [commandline parameters](#Commandline-Parameters))
3. Follow the instructions in the application to upload images/videos.
   It will ask for [permission](#Permission-to-access-your-Google-Photos-account) on first run.
4. Recommended: Remove the permission to access your Google Photos Account (see [Permissions](#Permission-to-access-your-Google-Photos-account)).

## Mac, Linux and Win10 User Guide
*Small, but dotnet core installation required.*
1. Install [.NET Core runtime version 2.2 or later](https://www.microsoft.com/net/download)
2. Download the latest **portable release** [here](https://github.com/runerasmussen/google-photos-upload/releases/latest)
3. Execute the application in a shell / command prompt: dotnet google-photos-upload.dll 
   (or use [commandline parameters](#Commandline-Parameters))
4. Follow the instructions in the application to upload images/videos.
   It will ask for [permission](#Permission-to-access-your-Google-Photos-account) on first run.
5. Recommended: Remove the permission to access your Google Photos Account (see [Permissions](#Permission-to-access-your-Google-Photos-account)).


## Things to note
 * Configuration option exists to prevent image upload if EXIF data is missing.
    In App.config set value to true (default value) to allow upload: `<add key="IMG_UPLOAD_NO_EXIF" value="true"/>`

 * The Google Photos Media title/description will be taken from the first available of these:
   1.  EXIF ImageDescription
   2. Filename without file extension

 * Supported file types: mov, avi, jpg, jpeg, gif
 
 * Ignored file types (other file types will give a warning message): txt, thm

 * Media will be uploaded in uncompressed 'Original Quality'. The Google Photos API does not support uploading in 'High Quality'. 
   To recover storage; Open [Google Photos Settings website](https://photos.google.com/settings) and 
   Click 'Recover Storage' (will downgrade ALL your photos from Original down to High Quality).


## Permission to access your Google Photos account
The tool will ask for your permission to access your Google Photos account (see table below) and will **not** share or use this access beyond your computer.

Permission | Used for
------------ | -------------
View your Google Photos library | Identify if an Album already exists
View the photos, videos and albums in your Google Photos | Identify available storage space in your Google Account
Add to your Google Photos library | Create new Albums and upload Image/Movie files

To revoke the permissions you should:
1. Remove the token file '.credentials/google-photos-upload.json'.
2. Remove the tool from your list of [Apps with access to your Google account](https://myaccount.google.com/permissions)


## Commandline Parameters
 * Win10: in a command prompt execute "dotnet google-photos-upload.exe -h"
 * Mac/Linux: in a shell execute "dotnet google-photos-upload.dll -h"

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
