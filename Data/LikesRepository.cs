using DatingApp.Entities;
using DatingApp.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Data;

public class LikesRepository(AppDbContext context) : ILikesRepository
{
    public async Task<MemberLike?> GetMemberLike(string sourceMemberId, string targetMemberId)
    {
        return await context.Likes.FindAsync(sourceMemberId, targetMemberId);
    }

    public async Task<IReadOnlyList<Member>> GetMemberLikes(string predicate, string memberId)
    {
        var query = context.Likes.AsQueryable();

        switch (predicate)
        {
            case "liked" :
                return await query
                    .Where(m => m.SourceMemberId == memberId)
                    .Select(x => x.TargetMember)
                    .ToListAsync();
            case "likedBy":
                return await query
                    .Where(m => m.TargetMemberId == memberId)
                    .Select(x => x.SourceMember)
                    .ToListAsync();
            default:
                var likedIds = await GetCurrentMemberLikeIds(memberId);
                return await query
                    .Where(x => x.TargetMemberId == memberId && likedIds.Contains(x.SourceMemberId))
                    .Select(x => x.SourceMember)
                    .ToListAsync();
        }
    }

    public async Task<IReadOnlyList<string>> GetCurrentMemberLikeIds(string memberId)
    {
        return await context.Likes
            .Where(x => x.SourceMemberId == memberId)
            .Select(x => x.TargetMemberId).ToListAsync();
    }

    public void DeleteLike(MemberLike like)
    {
        context.Likes.Remove(like);
    }

    public void AddLike(MemberLike like)
    {
        context.Likes.Add(like);
    }

    public async Task<bool> SaveAllChanges()
    {
       return await context.SaveChangesAsync() > 0;
    }
}