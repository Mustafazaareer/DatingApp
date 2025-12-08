using System.Security.Cryptography;
using System.Text; 
using DatingApp.Data;
using DatingApp.Dtos;
using DatingApp.Entities;
using DatingApp.Extensions;
using DatingApp.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Controllers;

public class AccountController(UserManager<AppUser> userManager,ITokenService tokenService) :BaseController
{
    [HttpPost("register")] // api/acocunt/register
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        
        var user = new AppUser
        {
            Name = registerDto.Name,
            Email = registerDto.Email,
            UserName = registerDto.Email,
            Member = new Member
            {
                DisplayName = registerDto.Name,
                City = registerDto.City,
                Country = registerDto.Country,
                DateOfBirth = registerDto.DateOfBirth,
                Gender = registerDto.Gender
            }
        };
        var result = await userManager.CreateAsync(user,registerDto.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("identity",error.Description);
            }

            return ValidationProblem();
        }
        await SetRefreshTokenCookie(user);
        return await user.ToDto(tokenService);

    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> login([FromBody]LoginDto loginDto)
    {
        var user = await userManager.FindByEmailAsync(loginDto.Email);
        if (user==null) return Unauthorized("Invalid Email Address");
        var result = await userManager.CheckPasswordAsync(user,loginDto.Password);
        if (!result) return Unauthorized("Invalid Password");
        await SetRefreshTokenCookie(user);
        return await user.ToDto(tokenService);
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<UserDto>> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (refreshToken == null) return NoContent();

        var user = await userManager.Users.FirstOrDefaultAsync(x =>
            x.RefreshToken == refreshToken && x.RefreshTokenExpiry > DateTime.UtcNow);
        if (user == null) return Unauthorized();

        await SetRefreshTokenCookie(user);
        return await user.ToDto(tokenService);
    }

    private async Task SetRefreshTokenCookie(AppUser appUser)
    {
        var refreshToken = tokenService.CreateRefreshToken();
        appUser.RefreshToken = refreshToken;
        appUser.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await userManager.UpdateAsync(appUser);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        };
        
        Response.Cookies.Append("refreshToken",refreshToken,cookieOptions);

    }
    
    
    
}