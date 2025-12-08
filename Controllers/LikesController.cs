using DatingApp.Data;
using DatingApp.Dtos;
using DatingApp.Entities;
using DatingApp.Extensions;
using DatingApp.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Controllers;

public class LikesController(ILikesRepository likesRepository):BaseController
{
    [HttpPost("{targetMemberId}")]
    public async Task<ActionResult<MemberDto>> ToggleLike(string targetMemberId)
    {
        var sourceMemberId = User.GetMemberId();
        if (sourceMemberId == targetMemberId)
        {
            return BadRequest("You Cannot Liked YourSelf");
        }

        var existingLike = await likesRepository.GetMemberLike(sourceMemberId, targetMemberId);

        if (existingLike == null)
        {
            var like = new MemberLike
            {
                SourceMemberId = sourceMemberId,
                TargetMemberId = targetMemberId
            };
            likesRepository.AddLike(like);
        }
        else
        {
            likesRepository.DeleteLike(existingLike);

        }

        if (await likesRepository.SaveAllChanges()) return Ok();

        return BadRequest("Failed To Update Like");
    }

    [HttpGet("list")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetCurrentMemberLikeIds()
    {
        return Ok(await likesRepository.GetCurrentMemberLikeIds(User.GetMemberId()));
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Member>>> GetMemberLikes(string predicate)
    {
        return Ok( await likesRepository.GetMemberLikes(predicate,User.GetMemberId()));
    }

}