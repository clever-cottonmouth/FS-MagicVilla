using AutoMapper.Internal;
using MagicVilla_Utility;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_Web.Models;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static MagicVilla_Utility.SD;

namespace MagicVilla_Web.Services
{
    public class BaseService : IBaseService
    {
        public APIResponse responseModel { get; set; }
        public IHttpClientFactory httpClient { get; set; }
        private readonly ITokenProvider _tokenProvider;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IApiMessageRequestBuilder _messageRequestBuilder;
        protected readonly string VillaApiUrl;
        public BaseService(IHttpClientFactory httpClient, ITokenProvider tokenProvider, IConfiguration configuration, IHttpContextAccessor contextAccessor, IApiMessageRequestBuilder messageRequestBuilder)
        {
            this.responseModel = new();
            VillaApiUrl = configuration.GetValue<string>("ServiceUrls:VillaAPI");
            this.httpClient = httpClient;
            _tokenProvider = tokenProvider;
            _contextAccessor = contextAccessor;
            _messageRequestBuilder = messageRequestBuilder;
        }
        public async Task<T> SendAsync<T>(ApiRequest apiRequest, bool withBearer = true)
        {
            try
            {
                var client = httpClient.CreateClient("MagicAPI");

                var messageFactory = () =>
                {
                    return _messageRequestBuilder.Build(apiRequest);
                };

                HttpResponseMessage httpResponseMessage = null;

                //if (!string.IsNullOrEmpty(apiRequest.Token))
                //{
                //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiRequest.Token);
                //}

                httpResponseMessage = await SendWithRefreshTokenAsync(client, messageFactory, withBearer);

                APIResponse FinalApiResponse = new()
                {
                    IsSuccess = false,
                };

                //var apiContent = await apiResponse.Content.ReadAsStringAsync();
                try
                {
                    switch (httpResponseMessage.StatusCode)
                    {
                        case HttpStatusCode.NotFound:
                            FinalApiResponse.ErrorMessages = new List<string>() { "Not Found" };
                            break;
                        case HttpStatusCode.Forbidden:
                            FinalApiResponse.ErrorMessages = new List<string>() { "Access Denied" };
                            break;
                        case HttpStatusCode.Unauthorized:
                            FinalApiResponse.ErrorMessages = new List<string>() { "Unauthorized" };
                            break;
                        case HttpStatusCode.InternalServerError:
                            FinalApiResponse.ErrorMessages = new List<string>() { "Internal Server Error" };
                            break;
                        default :
                            var apiContent = await httpResponseMessage.Content.ReadAsStringAsync();
                            FinalApiResponse.IsSuccess = true;
                            FinalApiResponse = JsonConvert.DeserializeObject<APIResponse>(apiContent);
                            break;
                    }         
                    
                }
                catch (AuthException ae)
                {
                    throw;
                }
                catch (Exception e)
                {
                    FinalApiResponse.ErrorMessages = new List<string>() { "Error Encountered", e.Message.ToString() };
                }
                var res = JsonConvert.SerializeObject(FinalApiResponse);
                var returnObj = JsonConvert.DeserializeObject<T>(res);
                return returnObj;

            }
            catch (Exception e)
            {
                var dto = new APIResponse
                {
                    ErrorMessages = new List<string> { Convert.ToString(e.Message) },
                    IsSuccess = false
                };
                var res = JsonConvert.SerializeObject(dto);
                var APIResponse = JsonConvert.DeserializeObject<T>(res);
                return APIResponse;
            }
        }

        private async Task<HttpResponseMessage> SendWithRefreshTokenAsync(HttpClient httpClient,
            Func<HttpRequestMessage> httpRequestMessageFactory, bool withBearer = true
            )
        {
            if (!withBearer)
            {
                return await httpClient.SendAsync(httpRequestMessageFactory());
            }
            else
            {
                TokenDto tokenDto = _tokenProvider.GetToken();
                if (tokenDto != null && !string.IsNullOrEmpty(tokenDto.AccessToken))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenDto.AccessToken);
                }
                try
                {
                    var response = await httpClient.SendAsync(httpRequestMessageFactory());
                    if (response.IsSuccessStatusCode)
                    {
                        return response;
                    }
                    
                    if(!response.IsSuccessStatusCode && response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        await InvokeRefreshTokenEndpoint(httpClient, tokenDto.AccessToken, tokenDto.RefreshToken);
                        response = await httpClient.SendAsync(httpRequestMessageFactory());
                        return response;
                    }
                    return response;
                }
                catch (AuthException ae)
                {
                    throw;
                }

                catch (HttpRequestException httpRequestException)
                {
                    if (httpRequestException.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        await InvokeRefreshTokenEndpoint(httpClient, tokenDto.AccessToken, tokenDto.RefreshToken);
                        return await httpClient.SendAsync(httpRequestMessageFactory());
                    }
                    throw;
                }

            }

        }

        private async Task InvokeRefreshTokenEndpoint(HttpClient httpClient, string existingAccessToken, string existingRefreshToken)
        {
            HttpRequestMessage message = new();
            message.Headers.Add("Accept", "application/json");
            message.RequestUri = new Uri($"{VillaApiUrl}/api/{SD.CurrentAPIVersion}/UserAPI/refresh");
            message.Method = HttpMethod.Post;
            message.Content = new StringContent(JsonConvert.SerializeObject(new TokenDto()
            {
                AccessToken = existingAccessToken,
                RefreshToken = existingRefreshToken

            }), Encoding.UTF8,"application/json");

            var response = await httpClient.SendAsync(message);
            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<APIResponse>(content);

            if (apiResponse?.IsSuccess != true)
            {
                await _contextAccessor.HttpContext.SignOutAsync();
                _tokenProvider.ClearToken();
                throw new AuthException();
            }
            else
            {
                var tokenDataStr = JsonConvert.SerializeObject(apiResponse.Result);
                var tokenDto = JsonConvert.DeserializeObject<TokenDto>(tokenDataStr);

                if (tokenDto!= null && !string.IsNullOrEmpty(tokenDto.AccessToken))
                {
                    await SignInWithNewTokens(tokenDto);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",tokenDto.AccessToken);
                }
            }
        }

        private async Task SignInWithNewTokens(TokenDto tokenDto)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(tokenDto.AccessToken);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(u => u.Type == "unique_name").Value));
            identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));
            var principal = new ClaimsPrincipal(identity);
            await _contextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            _tokenProvider.SetToken(tokenDto);
        }

    }
}
