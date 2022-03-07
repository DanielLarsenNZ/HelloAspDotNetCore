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
        private readonly RedisCache _redisCache;
        public readonly Dictionary<string, object> _result = new Dictionary<string, object>();

        public IndexModel(IConfiguration config, ILogger<IndexModel> logger, RedisCache redisCache)
        {
            _config = config;
            _logger = logger;
            _redisCache = redisCache;
        }

        public async Task OnGet()
        {
            await GetUrls();
            await GetRedisCacheItems();
        }

        private async Task GetRedisCacheItems()
        {
            if (!_redisCache.IsConnected) return;
            const string key = "index_page_count";
            int count = await _redisCache.Get<int>(key);

            _result.Add("Redis Cache GET index_page_count", count);

            count++;
            await _redisCache.Set(key, count, TimeSpan.FromDays(1));
        }

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
