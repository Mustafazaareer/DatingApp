using System.ComponentModel.DataAnnotations;

namespace DatingApp.Dtos;

public class LoginDto
{
    [EmailAddress]
    public required string Email { get; set; }
    [MinLength(4)]
    public required string Password { get; set; }
}