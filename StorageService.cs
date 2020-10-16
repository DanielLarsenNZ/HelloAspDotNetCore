using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloAspDotNetCore
{
    public class StorageService
    {
        private static readonly Lazy<CloudBlobClient> _lazyClient = new Lazy<CloudBlobClient>(InitializeCloudBlobClient);

        private static CloudBlobClient CloudBlobClient => _lazyClient.Value;

        public static bool StorageIsConfigured()
        {
            return !string.IsNullOrEmpty(Startup.Configuration["Blob.Path"])
                && !string.IsNullOrEmpty(Startup.Configuration["Blob.StorageConnectionString"]);
        }

        public async static Task<string> GetBlobContent()
        {
            try
            {
                CloudBlobClient.DefaultRequestOptions = new BlobRequestOptions { MaximumExecutionTime = TimeSpan.FromSeconds(5) };
                // get from Blob
                ICloudBlob blob = await CloudBlobClient.GetBlobReferenceFromServerAsync(
                    new Uri($"{CloudBlobClient.BaseUri}{Startup.Configuration["Blob.Path"]}"));

                using (var stream = new MemoryStream())
                {
                    await blob.DownloadToStreamAsync(stream);
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
            string hostname = CloudBlobClient.BaseUri.Host;
            var ips = System.Net.Dns.GetHostAddresses(hostname);
            return $"{hostname} IP = {string.Join(',', ips.Select(ip => ip.ToString()).ToArray())}";
        }

        private static CloudBlobClient InitializeCloudBlobClient()
        {
            if (Startup.Configuration["Blob.StorageConnectionString"] == null)
                throw new InvalidOperationException("App Setting \"Blob.StorageConnectionString\" is not set.");

            var account = CloudStorageAccount.Parse(Startup.Configuration["Blob.StorageConnectionString"]);
            return account.CreateCloudBlobClient();
        }
    }
}
