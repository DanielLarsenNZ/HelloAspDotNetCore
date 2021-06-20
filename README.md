# Hello ASP.NET Core

A simple Hello World ASP.NET Core website with a couple of features for testing.

![Screenshot](docs/images/home_page.jpg)

[![Build Status](https://daniellarsennz.visualstudio.com/HelloAspDotNetCore/_apis/build/status/DanielLarsenNZ.HelloAspDotNetCore?branchName=dnsdebug)](https://daniellarsennz.visualstudio.com/HelloAspDotNetCore/_build/latest?definitionId=10&branchName=dnsdebug)

    docker pull daniellarsennz/helloaspdotnetcoredns

> See also [DanielLarsenNZ/HelloFunctionsDotNetCore](https://github.com/DanielLarsenNZ/HelloFunctionsDotNetCore)


## Pages

* `/` - (Index) Simple response that will return the contents of a Blob if configured in App Settings. Now also adds basic dns test using two dns resolution methods one using System.Net.DNS and one using DNSClient and a basic socket testing capabiltiy. This is to support testing and troubleshooting with regional vnet integration
* `/Cpu[?durationMs={durationMs}]` - Will use as much CPU as possible on a single thread for `durationMs`. `durationMs` is optional and defaults to 100ms. Example: `/Cpu?durationMs=250`

## App Settings

* `Colour` - Sets the background colour of the Footer
* `loader.io` The loader.io validation key. When this setting is present the app will respond to [loader.io](https://loader.io) host validation requests.
* `StartupDelaySeconds` - The number of seconds to delay the ASP.NET Core startup process (to simulate application startup). If this setting is missing, empty or not an integer, there will be no delay.
* `GetUrls` - A semicolon delimited list of URL's to get. If present, the page will request these URLs and return the reponse code for each request.

### Blob settings

When these two settings are present, the text content of a Blob will be displayed on the home page.

* `Blob.StorageConnectionString` - Storage Account Connection String
* `Blob.Path` - Path to a Blob to display the contents of on the home page



