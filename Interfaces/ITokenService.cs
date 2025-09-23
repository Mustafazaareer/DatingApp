using Task.Entities;

namespace Task.Interfaces;

public interface ITokenService
{
    string CreateToken(AppUser user);
}