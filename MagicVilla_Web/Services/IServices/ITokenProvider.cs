using MagicVilla_VillaAPI.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicVilla_Web.Services.IServices
{
    public interface ITokenProvider
    {
        void SetToken(TokenDto tokenDto);
        TokenDto? GetToken();
        void ClearToken();
    }
}
