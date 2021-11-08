using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CEPAggregator.Classes
{
    public abstract class Parser
    {
        protected string GetApi(string url, MediaTypeWithQualityHeaderValue mediaType)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(mediaType);

            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsStringAsync().Result;
            }
            else
            {
                return null;
            }
        }
    }
}
