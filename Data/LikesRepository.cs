using DatingApp.Entities;
using DatingApp.Interfaces;

namespace DatingApp.Data;

public class LikesRepository : ILikesRepository
{
    public Task<MemberLike> GetMemberLike(string sourceMemberId, string targetMemberId)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<Member>> GetMemberLikes(string predicate, string memberId)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<string>> GetCurrentMemberLikeIds(string memberId)
    {
        throw new NotImplementedException();
    }

    public void DeleteLike(MemberLike like)
    {
        throw new NotImplementedException();
    }

    public void AddLike(MemberLike like)
    {
        throw new NotImplementedException();
    }

    public Task<bool> SaveAllChanges()
    {
        throw new NotImplementedException();
    }
}