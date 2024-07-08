using MagicVilla_Utility;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_Web.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicVilla_Web.Services
{
    public class TokenProvider : ITokenProvider
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public TokenProvider(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public void ClearToken()
        {
            _contextAccessor.HttpContext?.Response.Cookies.Delete(SD.AccessToken);
        }

        public TokenDto? GetToken()
        {
            try
            {
                bool hasAccessToken = _contextAccessor.HttpContext.Request.Cookies.TryGetValue(SD.AccessToken, out string accessToken);
                TokenDto tokenDto = new()
                {
                    Token = accessToken
                };
                return hasAccessToken ? tokenDto : null;
            }
            catch (Exception)
            {

                return null;
            }
        }

        public void SetToken(TokenDto tokenDto)
        {
            var cookieOptions= new CookieOptions { Expires= DateTime.UtcNow.AddDays(60) };
            _contextAccessor.HttpContext?.Response.Cookies.Append(SD.AccessToken, tokenDto.Token, cookieOptions);
        }
    }
}
