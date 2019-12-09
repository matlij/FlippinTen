using Models.Constants;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Android.Net;

namespace FlippinTen.Repository
{
    public class GenericRepository : IGenericRepository
    {
        public async Task<T> GetAsync<T>(string requestUri)
        {
            //requestUri = "http://192.168.0.19:5000/api/gameplay/testplayer";

            using (var client = new HttpClient(/*new AndroidClientHandler()*/))
            {
                var response = await client.GetAsync(requestUri);
                if (!response.IsSuccessStatusCode)
                    return default;

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<T>(content);

                return result;
            }
        }

        //private HttpClient CreateHttpClient(string authToken)
        //{
        //    var httpClient = new HttpClient();
        //    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //    if (!string.IsNullOrEmpty(authToken))
        //    {
        //        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        //    }
        //    return httpClient;
        //}
    }
}
