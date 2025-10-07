using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Controllers;

public class ErrorController : BaseController
{
    [HttpGet("auth")]
    public IActionResult GetAuth()
    {
        return Unauthorized();
    }
    [HttpGet("not-found")]
    public IActionResult GetNotFound()
    {
        return NotFound();
    }
    [HttpGet("server-error")]
    public IActionResult GetServerError()
    {
        throw new Exception("It Was A Server Error !");
    }
    [HttpGet("bad-request")]
    public IActionResult GetBadRequest()
    {
        return BadRequest("It Was A Bad Request !");
    }
}