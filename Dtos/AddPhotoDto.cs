using System.ComponentModel.DataAnnotations;

namespace Task.Dtos;

public class AddPhotoDto
{
    public string id { get; set; }  // match expected name "id"
    
    [Required]
    public IFormFile file { get; set; }
    
}