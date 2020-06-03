# Hello ASP.NET Core

A simple Hello World ASP.NET Core website with a couple of features for testing.

[![Build Status](https://daniellarsennz.visualstudio.com/HelloAspDotNetCore/_apis/build/status/DanielLarsenNZ.HelloAspDotNetCore?branchName=master)](https://daniellarsennz.visualstudio.com/HelloAspDotNetCore/_build/latest?definitionId=10&branchName=master)

    docker pull daniellarsennz/helloaspdotnetcore

> See also [DanielLarsenNZ/HelloFunctionsDotNetCore](https://github.com/DanielLarsenNZ/HelloFunctionsDotNetCore)

## App Settings

* `Colour` - Sets the background colour of the Footer
* `loader.io` The loader.io validation key. When this setting is present the app will respond to [loader.io](https://loader.io) host validation requests.
* `StartupDelaySeconds` - The number of seconds to delay the ASP.NET Core startup process (to simulate application startup). If this setting is missing, empty or not an integer, there will be no delay.

### Blob settings

When these two settings are present, the text content of a Blob will be displayed on the home page.

* `Blob.StorageConnectionString` - Storage Account Connection String
* `Blob.Path` - Path to a Blob to display the contents of on the home page



