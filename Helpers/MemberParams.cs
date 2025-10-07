namespace DatingApp.Helpers;

public class MemberParams : PagingParams
{
    public string? CurrentMemberId { get; set; }
    public string? Gender { get; set; }
    public int MaxAge { get; set; } = 10;
    public int MinAge { get; set; } = 100;
    public string OrderBy { get; set; } = "lastActive";
}