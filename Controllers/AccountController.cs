using System.Security.Cryptography;
using System.Text;
using DatingApp.Data;
using DatingApp.Dtos;
using DatingApp.Entities;
using DatingApp.Extensions;
using DatingApp.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Controllers;

public class AccountController(AppDbContext context,ITokenService tokenService) :BaseController
{
    [HttpPost("register")] // api/acocunt/register
    public async Task<ActionResult<UserDto>> Register(RegisterDto regesterDto)
    {
        if (await EmailExiests(regesterDto.Email)) return BadRequest();
        
        using var hmac = new HMACSHA512();
        var user = new AppUser
        {
            Name = regesterDto.Name,
            Email = regesterDto.Email,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(regesterDto.Password)),
            PasswordSalt = hmac.Key,
            Member = new Member
            {
                DisplayName = regesterDto.Name,
                City = regesterDto.City,
                Country = regesterDto.Country,
                DateOfBirth = regesterDto.DateOfBirth,
                Gender = regesterDto.Gender
            }
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user.ToDto(tokenService);

    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> login([FromBody]LoginDto loginDto)
    {
        var user = await context.Users.SingleOrDefaultAsync(u => u.Email== loginDto.Email);
        if (user==null) return Unauthorized("Invalid Email Address");
        using var hmac = new HMACSHA512(user.PasswordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
        for (var i = 0; i < computedHash.Length; i++)
        {
            if (user.PasswordHash[i] != computedHash[i]) return Unauthorized("Invalid Password");
            
        }

        return user.ToDto(tokenService);
    }
    
    private async Task<bool> EmailExiests(string email)
    {
        var IsExiest =await  context.Users.AnyAsync(user => user.Email.ToLower() == email.ToLower());
        Console.WriteLine(IsExiest);
        return IsExiest;
    }
}