using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Task.Entities;
[Table(name:"photos")]

public class Photo
{
    public int Id { get; set; }
    public string URL { get; set; }
    public string? PublicID { get; set; }
    
    // Navigation Property :
    [JsonIgnore] // avoid cycles
    public Member Member { get; set; } = null!;
    public string MemberId { get; set; }
}