using Api.Dto;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController(UserRepository users, AppDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetUserAsync([FromQuery] Guid id, 
        CancellationToken cancellationToken)
    {
        var user = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
        
        if (user is null) throw new Exception(id + " user was not found!");
        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUserAsync([FromBody] UserDto request,
        CancellationToken cancellationToken)
    {
        var user = new User 
        { 
            Id = Guid.NewGuid(),
            Firstname = request.Firstname,
            Lastname = request.Lastname 
        };

        await context.Users.AddAsync(user, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        await users.CreateUserAsync(user.Id);
        return Ok(user.Id);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUserAsync([FromBody] User request,
        CancellationToken cancellationToken)
    {
        context.Users.Update(request);
        await context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}