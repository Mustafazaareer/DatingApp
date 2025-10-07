using System.ComponentModel.DataAnnotations;

namespace DatingApp.Dtos;

public class RegisterDto
{
    [Required] [MaxLength(200)] public string Name { get; set; } = "";
    [Required] [EmailAddress] public string Email { get; set; } = "";
    [Required] [MaxLength(100)] [MinLength(4)] public string Password { get; set; } = "";
    [Required]
    public string Gender { get; set; } = string.Empty;
    [Required]
    public string City { get; set; } = string.Empty;
    [Required]
    public string Country { get; set; } = string.Empty;
    [Required]
    public DateOnly DateOfBirth { get; set; }
    
}