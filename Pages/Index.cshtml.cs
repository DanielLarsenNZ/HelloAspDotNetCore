using AzureCacheRedisClient;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace HelloAspDotNetCore.Pages
{
    public class IndexModel : PageModel
    {
        private static readonly HttpClient _http = new HttpClient();
        private readonly IConfiguration _config;
        private readonly ILogger _logger;
        private readonly RedisDb _redisDb;
        public readonly Dictionary<string, object> _result = new Dictionary<string, object>();

        public IndexModel(IConfiguration config, ILogger<IndexModel> logger, RedisDb redisDb)
        {
            _config = config;
            _logger = logger;
            _redisDb = redisDb;
        }

        public async Task OnGet()
        {
            await GetUrls();
            await GetRedisCacheItems();
        }

        private async Task GetRedisCacheItems()
        {
            if (!_redisDb.IsConnected) return;
            
            const string itemKey = "HelloAspDotNet_CacheItem";

            _result.Add("Redis Increment index_page_count", await _redisDb.Increment("index_page_count"));

            int itemSizeBytes = 1024;
            if (!string.IsNullOrWhiteSpace(_config["Redis:ItemSizeBytes"])) int.TryParse(_config["Redis:ItemSizeBytes"], out itemSizeBytes);

            int ttlSeconds = 60;
            if (!string.IsNullOrWhiteSpace(_config["Redis:TtlSeconds"])) int.TryParse(_config["Redis:TtlSeconds"], out ttlSeconds);

            string data = await _redisDb.Get<string>(itemKey);

            if (data == null)
            {
                // Construct a random string of data
                data = string.Empty;
                var random = new Random();
                for (int j = 0; j < itemSizeBytes; j++)
                {
                    data += (char)random.Next(65, 90);
                }

                await _redisDb.Set(itemKey, data, TimeSpan.FromSeconds(ttlSeconds)).ConfigureAwait(true);
                _result.Add($"Redis MISS Get, Set {itemKey}", Truncate(data, 20));
            }
            else
            {
                _result.Add($"Redis HIT Get {itemKey}", Truncate(data, 20));
            }
        }

        private string Truncate(string data, int length) => $"{data[..Math.Min(length, data.Length)]}... ({data.Length} Bytes)";

        private async Task GetUrls()
        {
            string urls = _config["GetUrls"];
            if (string.IsNullOrEmpty(urls))
            {
                _logger.LogInformation("Configuration setting \"GetUrls\" is not set");
                return;
            }

            foreach (string url in urls.Split(';')) await GetUrl(new Uri(url));
        }

        private async Task GetUrl(Uri uri)
        {
            //TODO: reusing HttpClient here is not very efficient?

            var responseResult = new Dictionary<string, object>();
            responseResult.Add("URL", uri.ToString());

            try
            {
                var response = await _http.GetAsync(uri);
                responseResult.Add("Response.StatusCode", response.StatusCode);
                responseResult.Add("Response.ReasonPhrase", response.ReasonPhrase);
                responseResult.Add("Response.IsSuccessStatusCode", response.IsSuccessStatusCode);
            }
            catch (Exception ex)
            {
                responseResult.Add("Exception.GetType().FullName", ex.GetType().FullName);
                responseResult.Add("Exception.Message", ex.Message);
            }

            _result.Add(uri.Host, responseResult);
        }
    }
}
