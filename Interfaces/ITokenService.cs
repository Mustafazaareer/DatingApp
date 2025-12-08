using DatingApp.Entities;
using Microsoft.AspNetCore.Identity;

namespace DatingApp.Interfaces;

public interface ITokenService
{
    Task<string> CreateToken(AppUser user);
    string CreateRefreshToken();
}