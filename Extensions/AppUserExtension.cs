using Task.Entities;
using Task.Interfaces;

namespace Task.Extensions;

public static class AppUserExtension
{
        public static UserDto ToDto(this AppUser user, ITokenService tokenService)
        {
            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Token = tokenService.CreateToken(user)
            };
        }
}