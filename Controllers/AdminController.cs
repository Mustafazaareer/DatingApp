using DatingApp.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Controllers;

public class AdminController(UserManager<AppUser> userManager) : BaseController
{
    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("user-with-roles")]
    public async Task<ActionResult> GetUsersWithRoles()
    {
        var  users = await userManager.Users.ToListAsync();
        var usersList =new List<object>();
        
        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            usersList.Add(new
            {
                user.Id,
                user.Email,
                Roles = roles.ToList()
            });
        }
        
        return Ok(usersList);
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPut("edit-roles/{userId}")]

    public async Task<ActionResult> GetRoles(string userId,[FromQuery]string roles)
    {
        if (string.IsNullOrEmpty(roles)) return BadRequest("Select At Least One Role");
        var selectedRoles = roles.Split(',').ToList();
        var user = await userManager.FindByIdAsync(userId);

        if (user == null) return BadRequest("Can't Find User!");

        var userRoles =await userManager.GetRolesAsync(user);

        var result = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

        if (!result.Succeeded) return BadRequest("Can't Update Roles");

        var checkResult = await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

        if (!checkResult.Succeeded) return BadRequest("Can't Remove Roles From User");

        return Ok(await userManager.GetRolesAsync(user));

    }
    
    
    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpGet("photos-to-moderate")]
    public ActionResult GetPhotosForModeration()
    {
        return Ok("Managers Can See This!");
    }

}