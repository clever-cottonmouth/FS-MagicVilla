using MagicVilla_Web.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicVilla_Web.Services.IServices
{
    public interface IAuthService
    {
        Task<T> LoginAsync<T>(LoginRequestDto objToCreate);
        Task<T> RegisterAsync<T>(RegisterationRequestDto objToCreate);
    }
}
