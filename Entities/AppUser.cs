using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DatingApp.Entities;

public class AppUser
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string Name { get; set; }
    public required string Email { get; set; }
    public string? ImageUrl { get; set; }
    public required byte[] PasswordHash { get; set; }
    public required byte[] PasswordSalt { get; set; }

    // Nav property
    [JsonIgnore]
    public Member Member { get; set; } = null!;
}