![Continuous Integration Build of Master branch](https://runerasmussen.visualstudio.com/google-photos-upload/_apis/build/status/CI%20Master%20Build)
[![SonarQube Static Code Analysis for quality and security](https://sonarcloud.io/api/project_badges/measure?project=runerasmussen_google-photos-upload&metric=alert_status)](https://sonarcloud.io/dashboard?id=runerasmussen_google-photos-upload)

![Works on all desktop operating systems](https://img.shields.io/badge/platform-windows%20%7C%20macos%20%7C%20linux-lightgrey.svg)
![Coded in .NET Core](https://img.shields.io/badge/.NET%20Core-2.2-lightgrey.svg)
![MIT License - free to use, without warranty!](https://img.shields.io/badge/license-MIT-green.svg)



Desktop tool to upload a local image directory into an Album in Google Photos, on pc/mac/linux.

Features:
* List current Albums in Google Photos
* Upload a directory into Google Photos as an Album
* Upload all subdirectories from a main directory into Google Photos as Albums





# User Guide
Ready to give it a try?<br />
Please note that this is a hobby project and comes without any warranty, but seem to work fine!

If you run into an issue then take a look at the log file found in log/google-photos-upload.log


### Permission to access your Google Photos account
The tool will ask for your permission to access your Google Photos account (see table below) and will **not** share or use this access beyond your computer.

Permission | Used for
------------ | -------------
View your Google Photos library | Identify if an Album already exists
View the photos, videos and albums in your Google Photos | Identify available storage space in your Google Account
Add to your Google Photos library | Create new Albums and upload Image/Movie files

To revoke the permissions you should:
1. Remove the token file '.credentials/google-photos-upload.json'.
2. Remove the tool from your list of [Apps with access to your Google account](https://myaccount.google.com/permissions)

### User Guide for Windows 10 (no requirement for dotnet core installation)
1. Download the latest release for your operating system [here](https://github.com/runerasmussen/google-photos-upload/releases/latest)
2. Execute the application:
   * On Windows 10: double click "google-photos-upload.exe"
   * For command parameter options in a command prompt execute "google-photos-upload.exe -h"
3. Follow the instructions in the application to upload images
4. Optional: Recover Storage; Open [Google Photos Settings website](https://photos.google.com/settings)
and Click 'Recover Storage' (will downgrade ALL your media from Original down to High Quality)
5. Optional: Remove the permission to access your Google Photos Account (see [Permissions](#Permission-to-access-your-Google-Photos-account)).

### User Guide for Mac, Linux and Win10 (smaller, but dotnet core installation required)
1. Install the [.NET Core runtime version 2.2 or later](https://www.microsoft.com/net/download)
2. Download the latest **portable release** [here](https://github.com/runerasmussen/google-photos-upload/releases/latest)
3. Execute the application:
   * On a shell / command prompt: dotnet google-photos-upload.dll
   * For command parameter options in a command prompt execute "dotnet google-photos-upload.dll -h"
4. Follow the instructions in the application to upload images
5. Optional: Recover Storage; Open [Google Photos Settings website](https://photos.google.com/settings)
and Click 'Recover Storage' (will downgrade ALL your photos from Original down to High Quality)
6. Optional: Remove the permission to access your Google Photos Account (see [Permissions](#Permission-to-access-your-Google-Photos-account)).



## Obtaining a Google Photos API key
Optional step! There is a key provided in the source code which has constraints, but sufficient for testing.
1. Obtain a Google Photos API key (Client ID and Client Secret) by following the instructions on [Getting started with Google Photos REST APIs](https://developers.google.com/photos/library/guides/get-started)

**NOTE** When selecting your application type in Step 4 of "Request an OAuth 2.0 client ID", please select "Other". There's also no need to carry out step 5 in that section.

2. Replace `YOUR_CLIENT_ID` in the client_id.json file with the provided Client ID. 
3. Replace `YOUR_CLIENT_SECRET` in the client_id.json file wiht the provided Client Secret.


# API references
* [Google Photos API v1](https://www.nuget.org/packages/Google.Apis.PhotosLibrary.v1/) / [Google Photos API homepage](https://developers.google.com/photos/) enables the API integration for Google Photos.
* [Six Labors ImageSharp](https://github.com/SixLabors/ImageSharp) reads Image Exif properties.
* [.NET Core](https://dot.net) as runtime engine.
* [NLog](https://nlog-project.org/) for logging.
* [Mono.Options](https://github.com/xamarin/XamarinComponents/tree/master/XPlat/Mono.Options) by the Xamarin team processes commandline arguments.


# Contribute
This is a hobby project. 
You are welcome to create an Issue or Pull Request if you have specific suggestions that can improve this small utility. 


# License
Licensed under the [MIT license](LICENSE.md). No warranty!
