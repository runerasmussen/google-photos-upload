![Build status](https://runerasmussen.visualstudio.com/google-photos-upload/_apis/build/status/google-photos-upload-CI)

# Introduction 
Unofficial Google Photos Uploader console application for Mac/PC/Linux.

Coded in .NET Core 2.1 and leveraging the [Google Photos API](https://developers.google.com/photos/).

Features:
* List current Google Photos Album
* Upload a Folder into Google Photos as an Album
* Upload Multiple Folders from a main Folder into Google Photos as Albums


# User Guide
Ready to give it a try on your Mac/PC/Linux?
1. This is hobby project software and comes without any warranty!
2. Install [.NET Core 2.1 or later](https://www.microsoft.com/net/download))
3. [Download the latest release](https://github.com/runerasmussen/google-photos-upload/releases/latest) of 'Google Photos Uploader
4. Execute the application on a shell / command prompt: dotnet google-photos-upload.dll
5. Follow the instructions in the application to upload images
6. Recover Storage: Open [Google Photos Settings website](https://photos.google.com/settings)
and Click 'Recover Storage' (will downgrade ALL your photos from Original down to High Quality)

For troubleshooting look at the log file found in log/google-photos-upload.log


# Developers
TODO: Guide users through getting the code up and running on their own system.

TODO: Describe and show how to build the code and run the tests.


### API references
* [Google Photos API v1](https://www.nuget.org/packages/Google.Apis.PhotosLibrary.v1/) / [Google Photos API homepage](https://developers.google.com/photos/) enables the API integration for Google Photos.
* [ExifLibrary](https://github.com/devedse/exiflibrary) reads Image Exif properties to ensure Image date is correct before upload.
* [.NET Core](https://dot.net)
* [NLog](https://nlog-project.org/)
* COMING SOON: [Mono.Options](https://github.com/xamarin/XamarinComponents/tree/master/XPlat/Mono.Options) by the Xamarin team processes commandline arguments.


### Contribute
This is a small hobby project. 
You are welcome to create an Issue or Pull Request if you have specific suggestions that can improve this small utility. 


# License
Licensed under the [MIT license](LICENSE.md). No warranty!
