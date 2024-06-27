using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Services.IServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MagicVilla_Web.Services
{
    public class BaseService : IBaseService
    {
        public APIResponse responseModel { get; set; }
        public IHttpClientFactory httpClient {  get; set; }
        public BaseService(IHttpClientFactory httpClient)
        {
            this.responseModel = new();
            this.httpClient = httpClient;
        }
        public async Task<T> SendAsync<T>(ApiRequest apiRequest)
        {
            try
            {
                if (apiRequest == null)
                {
                    throw new ArgumentNullException(nameof(apiRequest), "API request cannot be null.");
                }

                if (string.IsNullOrEmpty(apiRequest.Url))
                {
                    throw new ArgumentException("API request URL cannot be null or empty.", nameof(apiRequest.Url));
                }

                if (httpClient == null)
                {
                    throw new InvalidOperationException("HttpClient instance is not initialized.");
                }

                using (var client = httpClient.CreateClient("MagicAPI"))
                {
                    HttpRequestMessage message = new HttpRequestMessage
                    {
                        RequestUri = new Uri(apiRequest.Url)
                    };

                    // Set request method
                    switch (apiRequest.ApiType)
                    {
                        case SD.ApiType.POST:
                            message.Method = HttpMethod.Post;
                            break;
                        case SD.ApiType.PUT:
                            message.Method = HttpMethod.Put;
                            break;
                        case SD.ApiType.DELETE:
                            message.Method = HttpMethod.Delete;
                            break;
                        default:
                            message.Method = HttpMethod.Get;
                            break;
                    }

                    // Set request content if there is data
                    if (apiRequest.Data != null)
                    {
                        message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data),
                                                            Encoding.UTF8,
                                                            "application/json");

                        // Set 'Content-Type' header correctly
                        if (apiRequest.ApiType != SD.ApiType.GET)
                        {
                            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        }
                    }

                    // Send the request
                    HttpResponseMessage apiResponse = await client.SendAsync(message);

                    // Handle response
                    apiResponse.EnsureSuccessStatusCode(); // Ensure success status code

                    var apiContent = await apiResponse.Content.ReadAsStringAsync();
                    var APIResponse = JsonConvert.DeserializeObject<T>(apiContent);
                    return APIResponse;
                }
            }
            catch (Exception e)
            {
                // Handle exceptions
                var dto = new APIResponse
                {
                    ErrorMessages = new List<string> { e.Message },
                    IsSuccess = false
                };
                var res = JsonConvert.SerializeObject(dto);
                var APIResponse = JsonConvert.DeserializeObject<T>(res);
                return APIResponse;
            }
        }


    }
}
