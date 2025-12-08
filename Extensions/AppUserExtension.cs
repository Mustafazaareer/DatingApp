using DatingApp.Entities;
using DatingApp.Interfaces;

namespace DatingApp.Extensions;

public static class AppUserExtension
{
        public async static Task<UserDto> ToDto(this AppUser user, ITokenService tokenService)
        {
            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email!,
                Token = await tokenService.CreateToken(user)
            };
        }
}