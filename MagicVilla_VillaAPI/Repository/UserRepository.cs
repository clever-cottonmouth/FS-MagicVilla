using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using MagicVillaVillaAPI.Migrations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MagicVilla_VillaAPI.Repository
{
    public class UserRepository : IUserRespository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private string secretKey;

        public UserRepository(ApplicationDbContext db, IConfiguration configuration, UserManager<ApplicationUser> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
            _userManager = userManager;
            _mapper = mapper;
            _roleManager = roleManager;
        }

        public bool IsUniqueUser(string username)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(x=>x.UserName == username);
            if (user == null) 
            {
                return true;
            }
            return false;
        }

        public async Task<TokenDto> Login(LoginRequestDto loginRequestDto)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u=>u.UserName.ToLower() == loginRequestDto.UserName.ToLower());
            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);
            if (user == null || isValid ==false) 
            {
                return new TokenDto()
                {
                    AccessToken = "",
                };
            }
            var jwtTokenId = $"JTI{Guid.NewGuid()}";
            var accessToken = await GetAccessToken(user, jwtTokenId);
            var refreshToken = await CreateNewRefreshToken(user.Id, jwtTokenId);
            TokenDto tokenDto = new()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,

            };
            return tokenDto;
        }

        public async Task<UserDto> Register(RegisterationRequestDto registerationRequestDto)
        {
            ApplicationUser user = new()
            {
                UserName = registerationRequestDto.UserName,
                Email = registerationRequestDto.UserName,
                NormalizedEmail= registerationRequestDto.UserName.ToUpper(),
                Name = registerationRequestDto.Name,
            };

            try
            {
                var result = await _userManager.CreateAsync(user, registerationRequestDto.Password);
                if (result.Succeeded) 
                {
                    if (!_roleManager.RoleExistsAsync(registerationRequestDto.Role).GetAwaiter().GetResult())
                    {
                        await _roleManager.CreateAsync(new IdentityRole(registerationRequestDto.Role));
                    }
                    await _userManager.AddToRoleAsync(user, registerationRequestDto.Role);
                    var userToReturn = _db.ApplicationUsers
                        .FirstOrDefault(u=>u.UserName == registerationRequestDto.UserName);
                    return _mapper.Map<UserDto>(userToReturn);
                }

            }
            catch (Exception)
            {

                throw;
            }

            return new UserDto();
        }

        public async Task<string> GetAccessToken(ApplicationUser user, string jwtTokenId)
        {
            //if user found generate token
            var roles = await _userManager.GetRolesAsync(user);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName.ToString()),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
                    new Claim(JwtRegisteredClaimNames.Jti, jwtTokenId),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id)
                }),
                Expires = DateTime.UtcNow.AddMinutes(60),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenStr = tokenHandler.WriteToken(token);
            return tokenStr;
        }

        public async Task<TokenDto> RefreshAccessToken(TokenDto tokenDto)
        {
            var existingRefreshToken = await _db.RefreshTokens.FirstOrDefaultAsync(u=>u.Refresh_Token == tokenDto.RefreshToken);
            if (existingRefreshToken == null) 
            {
                return new TokenDto();
            }
            var isTokenValid = GetAccessTokenData(tokenDto.AccessToken, existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);
            if (!isTokenValid) 
            {
                await MarkTokenAsInvalid(existingRefreshToken);
                return new TokenDto();
            }

            if (!existingRefreshToken.IsValid)
            {
                await MarkAllTokenInChainAsInvalid(existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);
            }

            if (existingRefreshToken.ExpiresAt < DateTime.UtcNow) 
            {
                await MarkTokenAsInvalid(existingRefreshToken);
                return new TokenDto();
            }

            var newRefreshToken = await CreateNewRefreshToken(existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);

            await MarkTokenAsInvalid(existingRefreshToken);

            var applicationUser = _db.ApplicationUsers.FirstOrDefault(u=>u.Id == existingRefreshToken.UserId);
            if (applicationUser == null) 
            {
                return new TokenDto();
            }
            var newAccessToken = await GetAccessToken(applicationUser, existingRefreshToken.JwtTokenId);

            return new TokenDto()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
            };

        }

        private async Task<string> CreateNewRefreshToken(string userId, string tokenId)
        {
            RefreshToken refreshToken = new()
            {
                IsValid = true,
                UserId = userId,
                JwtTokenId = tokenId,
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                Refresh_Token = Guid.NewGuid() + "-"+ Guid.NewGuid(),
            };
            await _db.RefreshTokens.AddAsync(refreshToken);
            await _db.SaveChangesAsync();
            return refreshToken.Refresh_Token;
        }

        private bool GetAccessTokenData(string accessToken, string expectedUserId, string expectedTokenId)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwt = tokenHandler.ReadJwtToken(accessToken);
                var jwtTokenId = jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Jti).Value;
                var userId = jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value;
                return userId == expectedUserId && jwtTokenId == expectedTokenId;
            }
            catch
            {
                return false;
            }
        }

        private async Task MarkAllTokenInChainAsInvalid(string userId, string tokenId)
        {
            await _db.RefreshTokens.Where(u => u.UserId == userId
            && u.JwtTokenId == tokenId)
                 .ExecuteUpdateAsync(u => u.SetProperty(refreshtoken => refreshtoken.IsValid, false));
        }

        private Task MarkTokenAsInvalid(RefreshToken refreshToken)
        {
            refreshToken.IsValid = false;
            return _db.SaveChangesAsync();
        }

        public async Task RevokeRefreshToken(TokenDto tokenDto)
        {
            var existingRefreshToken = await _db.RefreshTokens.FirstOrDefaultAsync(_ => _.Refresh_Token == tokenDto.RefreshToken);
            if (existingRefreshToken == null) 
            {
                return;
            }
            var isTokenValid = GetAccessTokenData(tokenDto.AccessToken, existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);
            if (!isTokenValid)
            {
                return;
            }
            await MarkAllTokenInChainAsInvalid(existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);
        }
    }
}
