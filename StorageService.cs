using Azure.Storage;
using Azure.Storage.Blobs;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloAspDotNetCore
{
    public class StorageService
    {
        private static readonly Lazy<BlobServiceClient> _lazyClient = new Lazy<BlobServiceClient>(InitializeCloudBlobClient);

        private static BlobServiceClient BlobServiceClient => _lazyClient.Value;

        public static bool StorageIsConfigured()
        {
            return !string.IsNullOrEmpty(Startup.Configuration["Blob.Path"])
                && !string.IsNullOrEmpty(Startup.Configuration["Blob.StorageConnectionString"]);
        }

        public async static Task<string> GetBlobContent()
        {
            try
            {
                if (string.IsNullOrEmpty(Startup.Configuration["Blob.Path"])) throw new InvalidOperationException("App Setting Blob.Path is not set.");

                var parts = Startup.Configuration["Blob.Path"].Split('/');

                if (parts.Length < 2)
                {
                    throw new InvalidOperationException("App Setting Blob.Path is not valid. App Setting Blob.Path must contain a container name and a file path in this format: container/path/to/file");
                }

                

                var container = BlobServiceClient.GetBlobContainerClient(parts[0]);
                string filePath = string.Join('/', parts);
                var blob = container.GetBlobClient(filePath);
                


                //CloudBlobClient.DefaultRequestOptions = new BlobRequestOptions { MaximumExecutionTime = TimeSpan.FromSeconds(5) };
                
                
                using (var stream = new MemoryStream())
                {
                    await blob.DownloadToAsync(stream);
                    stream.Seek(0, SeekOrigin.Begin);
                    return Encoding.UTF8.GetString(stream.ToArray());
                }
            }
            catch (Exception ex)
            {
                // NOTE: This is Test code - do not do this in Production
                return ex.Message;
            }
        }

        public static string GetStorageServerIps()
        {
            string hostname = BlobServiceClient.Uri.Host;
            var ips = System.Net.Dns.GetHostAddresses(hostname);
            return $"{hostname} IP = {string.Join(',', ips.Select(ip => ip.ToString()).ToArray())}";
        }

        private static BlobServiceClient InitializeCloudBlobClient()
        {
            if (Startup.Configuration["Blob.StorageConnectionString"] == null)
                throw new InvalidOperationException("App Setting \"Blob.StorageConnectionString\" is not set.");

            return new BlobServiceClient(Startup.Configuration["Blob.StorageConnectionString"]);
        }
    }
}
