using Api.Dto;
using Domain.Models;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController(UserRepository usersGraph, AppDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        var usersList = await context.Users
            .AsNoTracking()
            .ToListAsync(cancellationToken: cancellationToken);
        
        return Ok(usersList);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, 
        CancellationToken cancellationToken)
    {
        var user = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
        
        if (user is null) throw new Exception(id + " user was not found!");
        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] UserDto request,
        CancellationToken cancellationToken)
    {
        var user = new User 
        { 
            Id = Guid.NewGuid(),
            Firstname = request.Firstname,
            Lastname = request.Lastname,
            Username = request.Username
        };

        await context.Users.AddAsync(user, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        await usersGraph.CreateUserAsync(user.Id);
        return Ok(user.Id);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateAsync([FromBody] User request,
        CancellationToken cancellationToken)
    {
        context.Users.Update(request);
        await context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}