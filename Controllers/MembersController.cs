using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task.Data;
using Task.Dtos;
using Task.Entities;
using Task.Interfaces;

namespace Task.Controllers;


public class MembersController(IMemberRepository memberRepository, IPhotoService photoService) : BaseController
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Member>>> GetMembers()
    {
        return Ok(await memberRepository.GetAllMembersAsync());

    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Member>> GetMember(string id)
    {
        var member = await memberRepository.GetMemberByIdAsync(id);
        if (member == null) return NotFound();
        return member;
    }

    [HttpGet("{id}/photos")]
    public async Task<ActionResult<IReadOnlyList<Photo>>> GetMemberPhotos(string id)
    {
        var photos = await memberRepository.GetAllMemberPhotosAsync(id);
        return Ok(photos);
    }

    [HttpPost("add-photo")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<Photo>> AddPhoto([FromForm] AddPhotoDto input)
    {
        var member = await memberRepository.GetMemberForUpdate(input.id);
        if (member == null) return BadRequest("Can't Update Member!");
        var result = await photoService.UploadPhotoAsync(input.file);
        if (result.Error != null) return BadRequest($"Cloudinary Error: {result.Error.Message}");

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

    public async Task<ActionResult> SetMainPhoto(int photoId, string memberId)
    {
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

    public async Task<ActionResult> DeletePhoto(int photoId, string memberId)
    {
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
            if (result.Error != null) return BadRequest($"Cloudinary Error: {result.Error.Message}");
        }

        member.Photos.Remove(photo);
        if (await memberRepository.SaveAllAsync()) return Ok();
        return BadRequest("Problem Happened When Deleting The Photo");
    }

}