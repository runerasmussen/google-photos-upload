# Google Photos Upload

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


## User Guide
Ready to give it a try? Check out the [User Guide](USER_GUIDE.md).


## SDK / API references
The tool is leveraging these frameworks/api's:
* [Google Photos API v1](https://www.nuget.org/packages/Google.Apis.PhotosLibrary.v1/) / [Google Photos API homepage](https://developers.google.com/photos/) enables the API integration for Google Photos.
* [Six Labors ImageSharp](https://github.com/SixLabors/ImageSharp) reads Image Exif properties.
* [.NET Core](https://dot.net) as runtime engine.
* [NLog](https://nlog-project.org/) for logging.
* [Mono.Options](https://github.com/xamarin/XamarinComponents/tree/master/XPlat/Mono.Options) by the Xamarin team processes commandline arguments.


## Contribute
This is a hobby project. 
You are welcome to [report an Issue](https://github.com/runerasmussen/google-photos-upload/issues) if you found a bug or 
have a suggestion that can improve this small utility.


# License
Licensed under the [MIT license](LICENSE.md). No warranty!
