using Mercury.Models;
using Mercury.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Mercury.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserRepository _users;

    public UsersController(UserRepository users)
    {
        _users = users;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserAsync([FromQuery] Guid id)
    {
        var user = await _users.GetUserAsync(id);
        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUserAsync([FromBody] UserDto request)
    {
        var user = new User 
        { 
            Id = Guid.NewGuid(),
            Firstname = request.Firstname,
            Lastname = request.Lastname 
        };
        
        await _users.CreateUserAsync(user);
        return NoContent();
    }

    public record UserDto(string Firstname, string Lastname);
}