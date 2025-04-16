using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using Gregatr.Domain.Entities;
using Microsoft.Playwright;

namespace Gregatr.Domain.Services
{
    internal static class Fetcher
    {
        //It's recommended to use HttpClient as a singleton to improve performance, especially for multiple requests to the same base URL.
        //Based on future needs consider using IHttpClientFactory or SocketsHttpHandler
        private static readonly HttpClient _client = new HttpClient();
        public static async Task<string> GetContentAsync(string url)
        {
            string relevantHTML = String.Empty; 
            
            var ContentURI = new Uri(url);
            try
            {

                //TODO: AK - test performance using stream instead of string and looking for Result with partial page html 
                // Perform HTTP request
                using (var response= await _client.GetAsync(ContentURI, HttpCompletionOption.ResponseHeadersRead))
                {
                     //responseTask.Wait();
                    //var response = responseTask.Result;
                    response.EnsureSuccessStatusCode(); // Ensure successful response

                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {

                        //TODO: move into ReferencesCOnfig
                        int chunkSize, subChunkSize;
                        if (url.Contains("rotten"))
                        {                            
                            chunkSize = 120 * 1024; // 90 KB
                            subChunkSize = 50 * 1024; // 10 KB
                        }
                        else
                        {
                            chunkSize = 240 * 1024; // 90 KB
                            subChunkSize = 50 * 1024; // 10 KB
                        }
                        // Seek to the position of the relevant part
                        //stream.Seek(chunkSize - subChunkSize, SeekOrigin.Begin);

                        byte[] chunkBytes = new byte[chunkSize];

                        int bytesReadTotal = 0;
                        while (bytesReadTotal < chunkSize)
                        {
                            int bytesRead = await stream.ReadAsync(chunkBytes, bytesReadTotal, chunkSize - bytesReadTotal);
                            if (bytesRead == 0) break; // End of stream
                            bytesReadTotal += bytesRead;
                        }

                        //Read the last 10 KB of data into the buffer
                        byte[] subChunkBytes = new byte[subChunkSize];

                        Array.Copy(chunkBytes, chunkSize - subChunkSize, subChunkBytes, 0, subChunkSize);


                        //Convert the read data to string (assuming UTF-8 encoding)
                        relevantHTML = Encoding.UTF8.GetString(subChunkBytes);                      
                    }
                }
            }
            catch (HttpRequestException ex)
            {

                //System.Diagnostics.Debug.WriteLine($"{ContentURI} -- {ex.Message}");
                ContentURI = null;
                var message = ex.Message;
                //TODO: AK - test for ex.Response
                //TODO: AK - catch connectivity issues
                // Handle WebException ex (catching more general exception might be better for async code)
                if (ex.StatusCode != HttpStatusCode.NotFound) throw;
                //ignore 404 //"NotFound" this URI is invalid
                //this URI is invalid
                //_content = null;

                
            }
            return relevantHTML;
        }
        //NO CHUNKS code, also see Aggregator.Fetcher ajax chunks

        //using var responseStream = _client.GetStreamAsync(ContentURI);
        //response.EnsureSuccessStatusCode();s
        //    _content.Load(responseStream.);
        //    using (var stream = await response.IsCompleted.)
        //    {
        //        _content.Load(stream);
        //    }
        //}
        //try
        //{
        //    using var response = await _client.GetAsync(ContentURI);
        //    response.EnsureSuccessStatusCode();
        //    _content = new HtmlDocument();
        //    using (var stream = await response.Content.ReadAsStreamAsync())
        //    {
        //        _content.Load(stream);
        //    }
        //}

        //OLD HttpWebRequest code:
        //    try
        //    {
        //        var request = (HttpWebRequest)WebRequest.Create(ContentURI);
        //        var response = (HttpWebResponse)request.GetResponse();

        //        _content = new HtmlDocument();
        //        using (var stream = response.GetResponseStream())
        //        {
        //            _content.Load(stream);
        //        }
        //    }


        // Get the SyndicationFeed from a file path
        public static SyndicationFeed GetSourceFeed(string filePath)
        {
            // Using block ensures reader is disposed after use
            using (var reader = XmlReader.Create(filePath))
            {
                return SyndicationFeed.Load(reader);
            }
        }

        // Get the SyndicationFeed from a URI

        public static SyndicationFeed GetSourceFeed(Uri feedUri)
        {
            // Using block ensures reader is disposed after use
            using (var reader = XmlReader.Create(feedUri.ToString()))
            {
                return SyndicationFeed.Load(reader);
            }
        }

        public static async Task<string> GetContentPlaywriteAsync(string url)
        {
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });

            var page = await browser.NewPageAsync();

            var context = await browser.NewContextAsync(new BrowserNewContextOptions
            {
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
                            "(KHTML, like Gecko) Firefox/110.0 Safari/537.36",
                ViewportSize = new() { Width = 1280, Height = 800 }
            });

            
            await page.GotoAsync(url);

            // Simulate human-like delay
            await page.WaitForTimeoutAsync(5000);
            // Wait for the checkbox to be available and visible
            
            await page.WaitForSelectorAsync("input[type='checkbox']", new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible }
);
            // Wait for the checkbox to be visible
            var checkbox = await page.QuerySelectorAsync("div.cb-c label.cb-lb input[type='checkbox']");

            // Ensure the checkbox is visible
            await checkbox.WaitForElementStateAsync(ElementState.Visible);

            // Simulate clicking the checkbox
            await checkbox.ClickAsync();

            // Optionally, wait for the CAPTCHA verification to complete
            await page.WaitForTimeoutAsync(5000);  // Wait for 5 seconds (adjust if needed)

            // Get the page's HTML content
            string pageHtml = await page.ContentAsync();
            //await page.WaitForTimeoutAsync(5000); // observe results
            //await browser.CloseAsync();
            return pageHtml;
        }

    }
}
