using DatingApp.Dtos;
using DatingApp.Entities;
using DatingApp.Extensions;
using DatingApp.Helpers;
using DatingApp.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Member = AutoMapper.Execution.Member;

namespace DatingApp.Controllers;


public class MembersController(IMemberRepository memberRepository, IPhotoService photoService) : BaseController
{
    [HttpGet]
    public async Task<ActionResult<Member>> GetMembers([FromQuery] MemberParams memberParams)
    {
        memberParams.CurrentMemberId = User.GetMemberId();
        return Ok(await memberRepository.GetMembersAsync(memberParams));

    }

    [HttpGet("{id}")] // locahost:5001/api/members/bob-id
    public async Task<ActionResult<Member?>> GetMember(string id)
    {
        var member = await memberRepository.GetMemberByIdAsync(id);

        if (member == null) return NotFound();

        return Ok(member);
    }

    [HttpGet("{id}/photos")]
    public async Task<ActionResult> GetMemberPhotos(string id)
    {
        var photos = await memberRepository.GetPhotosForMemberAsync(id);
        return Ok(photos);
    }

    [HttpPost("add-photo")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<Photo>> AddPhoto([FromForm] AddPhotoDto input)
    {
        var member = await memberRepository.GetMemberForUpdate(input.id);
        if (member == null) return BadRequest("Can't Update Member!");
        var result = await photoService.UploadPhotoAsync(input.file);
        if (result.Error != null) return BadRequest($"Cloudinary Error: ");

        var photo = new Photo
        {
            PublicID = result.PublicId,
            URL = result.Url.AbsoluteUri,
            MemberId = member.Id,
        };
        if (member.ImageUrl == null)
        {
            member.AppUser.ImageUrl = photo.URL;
            member.ImageUrl = photo.URL;
        }

        member.Photos.Add(photo);
        if (await memberRepository.SaveAllAsync()) return photo;

        return BadRequest("Problem When Upload Photo");
    }

    [HttpPost("set-main-photo")]

    public async Task<ActionResult> SetMainPhoto(int photoId)
    
    {
        var memberId = User.GetMemberId();
        var member = await memberRepository.GetMemberForUpdate(memberId);
        if (member == null) return BadRequest("Can't Update Member!");
        var photo = member.Photos.SingleOrDefault(p => p.Id == photoId);

        if (member.ImageUrl == photo?.URL || photo == null)
        {
            return BadRequest("Can't Set This As Main Photo");
        }

        member.ImageUrl = photo.URL;
        member.ImageUrl = photo.URL;

        if (await memberRepository.SaveAllAsync()) return Ok();

        return BadRequest("Problem Setting Main Photo");
    }


    [HttpDelete("delete-photo/{photoId}")]

    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        var memberId = User.GetMemberId();
        var member = await memberRepository.GetMemberForUpdate(memberId);
        if (member == null) return BadRequest("Can't Update Member!");
        var photo = member.Photos.SingleOrDefault(p => p.Id == photoId);
        
        if ( photo.URL == member.ImageUrl || photo == null)
        {
            return BadRequest("This Photo Can't Be Deleted");
        }
        
        if(photo.PublicID != null)
        {
            var result = await photoService.DeletePhotoAsync(photo.PublicID);
            if (result.Error != null) return BadRequest($"Cloudinary Error:");
        }

        member.Photos.Remove(photo);
        if (await memberRepository.SaveAllAsync()) return Ok();
        return BadRequest("Problem Happened When Deleting The Photo");
    }

    [HttpPut]
    public async Task<ActionResult> UpdateMember(MemberUpdateDto memberUpdateDto)
    {
        var memberId = User.GetMemberId();

        var member = await memberRepository.GetMemberForUpdate(memberId);

        if (member == null) return BadRequest("Could not get member");

        member.DisplayName = memberUpdateDto.DisplayName ?? member.DisplayName;
        member.Description = memberUpdateDto.Description ?? member.Description;
        member.City = memberUpdateDto.City ?? member.City;
        member.Country = memberUpdateDto.Country ?? member.Country;

        member.AppUser.Name = memberUpdateDto.DisplayName ?? member.AppUser.Name;

        memberRepository.Update(member); // optional

        if (await memberRepository.SaveAllAsync()) return NoContent();

        return BadRequest("Failed to update member");
    }
}