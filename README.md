# Hello ASP.NET Core

A simple Hello World ASP.NET Core website with a couple of features for testing.

![Screenshot](docs/images/home_page.jpg)

[![docker_push](https://github.com/DanielLarsenNZ/HelloAspDotNetCore/actions/workflows/main.yml/badge.svg)](https://github.com/DanielLarsenNZ/HelloAspDotNetCore/actions/workflows/main.yml)

> Try it out: <https://helloaspnet-eus2.azurewebsites.net>

> See also [DanielLarsenNZ/HelloFunctionsDotNetCore](https://github.com/DanielLarsenNZ/HelloFunctionsDotNetCore)

## Getting started

With [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli):

```
az group create -n helloaspnet-rg -l westus2
az appservice plan create -g helloaspnet-rg -n helloaspnet-plan --is-linux --sku FREE
az webapp create -g helloaspnet-rg -p helloaspnet-plan -n hello-aspnet -i daniellarsennz/helloaspdotnetcore
```

## App Settings

* `Colour` - Sets the background colour of the Footer
* `loader.io` The loader.io validation key. When this setting is present the app will respond to [loader.io](https://loader.io) host validation requests.
* `StartupDelaySeconds` - The number of seconds to delay the ASP.NET Core startup process (to simulate application startup). If this setting is missing, empty or not an integer, there will be no delay.
* `GetUrls` - A semicolon delimited list of URL's to get. If present, the page will request these URLs and return the reponse code for each request.

> **Note**: Nested App Settings in JSON config have dots in their names that are not compatible with environment variables, and therefore Azure App Service App Settings. For these App Setting names, substitute the dot with two underscores, i.e. `Blob.StorageConnectionString` is equivalent to `Blob__StorageConnectionString`.

### Blob settings

When these two settings are present, the text content of a Blob will be displayed on the home page.

* `Blob.StorageConnectionString` - Storage Account Connection String
* `Blob.Path` - Path to a Blob in the format `container/path/to/file` to display the contents of on the home page

### Redis settings

When `Redis.ConnectionString` is present, the app will increment a `index_page_count` key value and then get a `HelloAspDotNet_CacheItem`. If no cache item is found, a random string value will be set.

* `Redis.ConnectionString` - Connection string for a Redis server. When present, the web app will connect and send  operations to Redis.
* `Redis.ItemSizeBytes` - The app will generate a random string payload of this many bytes to get and set. The default is 1024.
* `Redis.TtlSeconds` - The expiry time for the item in seconds. Default is 60.