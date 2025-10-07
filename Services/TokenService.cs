using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DatingApp.Entities;
using DatingApp.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.Services;

public class TokenService(IConfiguration config) : ITokenService
{
    public string CreateToken(AppUser user)
    {
        var tokenKey = config["TokenKey"] ?? throw new Exception("Cannot Get Token Key");
        if (tokenKey.Length < 64) throw new Exception("Your Token Key Need To Be >= characters");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
        };
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = creds

        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    
    public string CreateTokenTest(AppUser user)
    {
        var tokenKey = "";
        if (config["TokenKey"].Length <0)
        {
            throw new Exception("Token Key not Exiest !");
        }

        tokenKey = config["TokenKey"];
        if (tokenKey!.Length < 64) throw new Exception("Token Key Must Be More The 64 Characters");
        var kew =new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

        var clams = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier , user.Name),
            new Claim(ClaimTypes.Email,user.Email)
        };

        var creds = new SigningCredentials(kew, SecurityAlgorithms.HmacSha512Signature);

        var tokenDesc =new SecurityTokenDescriptor
        {
            Subject =new ClaimsIdentity(clams),
            SigningCredentials = creds,
            Expires = DateTime.UtcNow.AddDays(7)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDesc));

    }

    public string CreateTokenTestTwo(AppUser user)
    {
        var tokenKey = config["TokenKey"] ?? throw new Exception("Token key Not Exist !");
        if (tokenKey.Length < 64) throw new Exception("Token Key Too Short, Must Be More Than 64 Characters");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier,user.Name),
            new Claim(ClaimTypes.Email,user.Email)
        };

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDesc = new SecurityTokenDescriptor
        {
            Subject =new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDesc);
        return tokenHandler.WriteToken(token);
    }

}











