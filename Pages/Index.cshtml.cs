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
        public string dnsResult;
        public string netDnsResult;
        public TestSocketResult testConnetionResult;

        public string error;

        [BindProperty(SupportsGet =true)]
        public string Hostname { get; set; }
        [BindProperty(SupportsGet =true)]
        public string Server { get; set; }
        [BindProperty(SupportsGet =true)]
        public string NameServer { get; set; }
        [BindProperty(SupportsGet =true)]
        public bool UseSpecificNS { get; set; }
        [BindProperty(SupportsGet =true)]
        public int Port { get; set; }

        public IndexModel(IConfiguration config, ILogger<IndexModel> logger)
        {
            _config = config;
            _logger = logger;
            Hostname = "www.microsoft.com";
            Port = 443;
            Server = "www.microsoft.com";
        }
        public void OnPost()
        {
            error = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Hostname))
                {

                    if (UseSpecificNS)
                    {
                        dnsResult = DNSService.GetHostAddresses(Hostname, NameServer, this._logger);
                    }
                    else
                    {
                        dnsResult = DNSService.GetHostAddresses(Hostname, "", this._logger);
                    }

                    netDnsResult = DNSService.GetNetDNSHostAddresses(Hostname);

                }
                if (!String.IsNullOrEmpty(Server) && Port > 0)
                {
                    testConnetionResult = DNSService.TestConnection(Server, Port, this._logger);
                }
            }
            catch (Exception e)
            {
                error = e.Message;
            }

        }
        public async Task OnGet()
        {
            OnPost();
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
