using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task.Data;
using Task.Entities;

namespace Task.Controllers;

public class UserController : BaseController
{
    private readonly AppDbContext _context;

    public UserController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<List<AppUser>> GetUsers()
    {
        var users =  _context.Users.ToList();
        return Ok(users);
    }
    [Authorize]
    [HttpGet("{id}")]
    public ActionResult<AppUser> GetUser(string id)
    {
        var user = _context.Users.FindAsync(id);
        if (user == null) return NotFound();
        return Ok(user.Result);
    }
    
    [HttpGet("Async")]
    public async Task<ActionResult<List<AppUser>>> GetUsersAsync()
    {
        var users = await _context.Users.ToListAsync();
        return Ok(users);
    }
    
    [HttpGet("Async/{id}")]
    public async Task<ActionResult<AppUser>> GetUserAsync(string id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();
        return Ok(user);
    }
}