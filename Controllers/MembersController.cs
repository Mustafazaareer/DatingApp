using System.Security.Claims;
using AutoMapper;
using DatingApp.Dtos;
using DatingApp.Entities;
using DatingApp.Extensions;
using DatingApp.Helpers;
using DatingApp.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Controllers;

  [Authorize]
    public class MembersController(IMemberRepository memberRepository,
        IPhotoService photoService, IMapper mapper) : BaseController
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Member>>> GetMembers(
            [FromQuery] MemberParams memberParams)
        {
            memberParams.CurrentMemberId = User.GetMemberId();

            return Ok(await memberRepository.GetMembersAsync(memberParams));
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<MemberDto>> GetMember(string id)
        {
            var member = await memberRepository.GetMemberByIdAsync(id);

            if (member == null) return NotFound();
            var result = mapper.Map<MemberDto>(member);

            return Ok(result);
        }

        [HttpGet("{id}/photos")]
        public async Task<ActionResult<IReadOnlyList<Photo>>> GetMemberPhotos(string id)
        {
            return Ok(await memberRepository.GetPhotosForMemberAsync(id));
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

 
        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto([FromForm] AddPhotoDto addPhotoDto)
        {
            var member = await memberRepository.GetMemberForUpdate(User.GetMemberId());

            if (member == null) return BadRequest("Cannot update member");

            var result = await photoService.UploadPhotoAsync(addPhotoDto.file);

            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                URL = result.SecureUrl.AbsoluteUri,
                PublicID = result.PublicId,
                MemberId = User.GetMemberId()
            };

            if (member.ImageUrl == null)
            {
                member.ImageUrl = photo.URL;
                member.AppUser.ImageUrl = photo.URL;
            }

            member.Photos.Add(photo);

            if (await memberRepository.SaveAllAsync())
            {
                var photoDto = mapper.Map<PhotoDto>(photo);

                return photoDto;
            }

            return BadRequest("Problem adding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var member = await memberRepository.GetMemberForUpdate(User.GetMemberId());

            if (member == null) return BadRequest("Cannot get member from token");
            var photo = member.Photos.SingleOrDefault(x => x.Id == photoId);
            if (member.ImageUrl == photo?.URL || photo == null)
            {
                return BadRequest("Cannot set this as main image");
            }

            member.ImageUrl = photo.URL;
            member.AppUser.ImageUrl = photo.URL;

            if (await memberRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Problem setting main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var member = await memberRepository.GetMemberForUpdate(User.GetMemberId());

            if (member == null) return BadRequest("Cannot get member from token");

            var photo = member.Photos.SingleOrDefault(x => x.Id == photoId);

            if (photo == null || photo.URL == member.ImageUrl)
            {
                return BadRequest("This photo cannot be deleted");
            }

            if (photo.PublicID != null)
            {
                var result = await photoService.DeletePhotoAsync(photo.PublicID);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            member.Photos.Remove(photo);

            if (await memberRepository.SaveAllAsync()) return Ok();

            return BadRequest("Problem deleting the photo");
        }
    }