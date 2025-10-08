using Newtonsoft.Json;

namespace DatingApp.Helpers;

public class MemberParams : PagingParams
{
    public string? Gender { get; set; }
    [JsonIgnore] 
    public string? CurrentMemberId { get; set; }
    public int MinAge { get; set; } = 18;
    public int MaxAge { get; set; } = 100;
    public string OrderBy { get; set; } = "lastActive";
}