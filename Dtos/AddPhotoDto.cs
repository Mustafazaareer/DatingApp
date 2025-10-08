using System.ComponentModel.DataAnnotations;

namespace DatingApp.Dtos;

public class AddPhotoDto
{
    [Required]
    public IFormFile file { get; set; }
    
}