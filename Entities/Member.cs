using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DatingApp.Entities;

public class Member
{
    public string Id { get; set; } = null!;
    public DateOnly DateOfBirth { get; set; }
    public string? ImageUrl { get; set; }
    public required string DisplayName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastActive { get; set; } = DateTime.UtcNow;
    public required string Gender { get; set; }
    public string? Description { get; set; }
    public required string City { get; set; }
    public required string Country { get; set; }

    // Navigation property
    [JsonIgnore]
    public string UserId { get; set; }
    [JsonIgnore]
    public List<Photo> Photos { get; set; } = [];
    
    [JsonIgnore]
    public List<MemberLike> LikedByMembers { get; set; } = [];
    
    [JsonIgnore]
    public List<MemberLike> LikedMembers { get; set; } = [];

    [JsonIgnore]
    [ForeignKey(nameof(Id))]
    public AppUser AppUser { get; set; } = null!;
}