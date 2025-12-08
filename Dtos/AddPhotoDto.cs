using System.ComponentModel.DataAnnotations;

namespace DatingApp.Dtos;

public class AddPhotoDto
{
    [Required]
    public required IFormFile file { get; set; }
    
}