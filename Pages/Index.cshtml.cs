using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HelloAspDotNetCore.Pages
{
    public class IndexModel : PageModel
    {
        private static readonly HttpClient _http = new HttpClient();
        private readonly IConfiguration _config;
        private readonly ILogger _logger;
        public readonly Dictionary<string, object> _result = new Dictionary<string, object>();

        public IndexModel(IConfiguration config, ILogger<IndexModel> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task OnGet()
        {
            await GetUrls();
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
