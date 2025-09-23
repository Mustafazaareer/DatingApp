using System.ComponentModel.DataAnnotations;

namespace Task.Dtos;

public class RegisterDto
{
    [Required] [MaxLength(200)] public string Name { get; set; } = "";
    [Required] [EmailAddress] public string Email { get; set; } = "";
    [Required] [MaxLength(100)] [MinLength(4)] public string Password { get; set; } = "";
}