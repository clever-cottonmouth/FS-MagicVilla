﻿using MagicVilla_Utility;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MagicVilla_Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDto obj = new();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequestDto obj)
        {
            APIResponse response = await _authService.LoginAsync<APIResponse>(obj);
            if(response != null && response.IsSuccess)
            {
                LoginResponseDto model = JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(response.Result));
                HttpContext.Session.SetString(SD.SessionToken, model.Token);
                return RedirectToAction("Index","Home");
            }
            else
            {
                ModelState.AddModelError("CustomError", response.ErrorMessages.FirstOrDefault());
                return View(obj);
            }
        }

        [HttpGet]
        public IActionResult Register() 
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterationRequestDto obj)
        {
            APIResponse result = await _authService.RegisterAsync<APIResponse>(obj);
            if (result != null && result.IsSuccess) 
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            HttpContext.Session.SetString(SD.SessionToken,"");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
