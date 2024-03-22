using Api.Dto;
using Domain.Models;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController(UserRepository users) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetUserAsync([FromQuery] Guid id)
    {
        var user = await users.GetUserAsync(id);
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
        
        await users.CreateUserAsync(user);
        return Ok(user.Id);
    }
}