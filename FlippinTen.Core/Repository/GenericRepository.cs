using FlippinTen.Core.Interfaces;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FlippinTen.Core.Repository
{
    public class GenericRepository : IGenericRepository
    {
        public async Task<T> GetAsync<T>(string requestUri)
        {
            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(requestUri);
                if (!response.IsSuccessStatusCode)
                    return default;

                var content = await response.Content.ReadAsStringAsync();
                T result = JsonConvert.DeserializeObject<T>(content);

                return result;
            }
        }

        public async Task<T> PostAsync<T>(string requestUri, T body)
        {
            using (var client = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(body));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = await client.PostAsync(requestUri, content);

                if (!response.IsSuccessStatusCode)
                    return default;

                var responseContent = await response.Content.ReadAsStringAsync();
                T result = JsonConvert.DeserializeObject<T>(responseContent);

                return result;
            }
        }
    }
}
