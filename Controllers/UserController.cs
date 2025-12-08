using DatingApp.Controllers;
using DatingApp.Data;
using DatingApp.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace DatingApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(AppDbContext context) : ControllerBase
{
    // [HttpPost]
    // public ActionResult<UserDto> CreateUser(UserDto user)
    // {
    //     var usr = new AppUser
    //     {       
    //         Name = user.Name,
    //         Email = user.Email,
    //         pa
    //     };
    //     context.Users.Add()
    // }
    
    [HttpGet]
    public ActionResult<List<AppUser>> GetUsers()
    {
        var users = context.Users.ToList();
        return Ok(users);
    }

    [Authorize]
    [HttpGet("{id}")]
    public ActionResult<AppUser> GetUser(string id)
    {
        var user = context.Users.Find(id); // sync Find
        if (user == null) return NotFound();
        return Ok(user);
    }

    [HttpGet("async")]
    public async Task<ActionResult<List<AppUser>>> GetUsersAsync()
    {
        var users = await context.Users.ToListAsync();
        return Ok(users);
    }

    [HttpGet("async/{id}")]
    public async Task<ActionResult<AppUser>> GetUserAsync(string id)
    {
        var user = await context.Users.FindAsync(id);
        if (user == null) return NotFound();
        return Ok(user);
    }
}