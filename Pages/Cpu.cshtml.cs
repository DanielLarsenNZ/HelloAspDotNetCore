using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Linq;
using System.Threading;

namespace HelloAspDotNetCore.Pages
{
    public class CpuModel : PageModel
    {
        /// <summary>
        /// Will use as much CPU as possible on a single thread for `durationMs`
        /// </summary>
        public void OnGet()
        {
            int.TryParse(Request.Query["durationMs"], out int durationMs);

            // default durationMs is 100ms
            durationMs = durationMs <= 0 ? 100 : durationMs;

            ViewData["durationMs"] = durationMs;

            var start = DateTimeOffset.UtcNow;
            var finish = start.Add(TimeSpan.FromMilliseconds(durationMs));

            // calculating primes just because it is fun. Probably doesn't use any more CPU than a simple loop
            for (long i = 0; i <= long.MaxValue ; i++)
            {
                bool isPrime = true; 
                for (long j = 2; j < i; j++) 
                {
                    if (i % j == 0) 
                    {
                        isPrime = false;
                        break;
                    }
                }
                if (isPrime)
                {
                    ViewData["maxPrime"] = i;
                }
                if (DateTimeOffset.UtcNow >= finish) break;
            }
        }
    }
}