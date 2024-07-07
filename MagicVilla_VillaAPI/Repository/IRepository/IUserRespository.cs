using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicVilla_VillaAPI.Repository.IRepository
{
    public interface IUserRespository
    {
        bool IsUniqueUser(string username);
        Task<TokenDto> Login(LoginRequestDto loginRequestDto);
        Task<UserDto> Register(RegisterationRequestDto registerationRequestDto);
    }
}
