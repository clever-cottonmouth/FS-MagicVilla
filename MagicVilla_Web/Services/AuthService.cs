using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicVilla_Web.Services
{
    public class AuthService :  IAuthService
    {
        private readonly IHttpClientFactory _clientFactory;
        private string villaUrl;
        private readonly IBaseService _baseService;

        public AuthService(IHttpClientFactory clientFactory, IConfiguration configuration, IBaseService baseService) 
        {
            _clientFactory = clientFactory;
            villaUrl = configuration.GetValue<string>("ServiceUrls:VillaAPI");
            _baseService = baseService;
        }

        public async Task<T> LoginAsync<T>(LoginRequestDto objToCreate)
        {
            return await _baseService.SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = objToCreate,
                Url = villaUrl + $"/api/{SD.CurrentAPIVersion}/UserAPI/login"
            });
        }

        public async Task<T> RegisterAsync<T>(RegisterationRequestDto objToCreate)
        {
            return await _baseService.SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = objToCreate,
                Url = villaUrl + $"/api/{SD.CurrentAPIVersion}/UserAPI/register"
            });
        }
    }
}
