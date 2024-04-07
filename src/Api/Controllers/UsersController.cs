using Api.Dto;
using Domain;
using Domain.Abstractions;
using Domain.Models;
using Infrastructure;
using Infrastructure.Repositories;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController(
    UserRepository usersGraph,
    AppDbContext context,
    FileRepository fileRepository) : ControllerBase
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
    public async Task<IActionResult> CreateAsync([FromBody] CreateUserDto request,
        CancellationToken cancellationToken)
    {
        var user = request.Adapt<User>();
        user.Id = Guid.NewGuid();

        await context.Users.AddAsync(user, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        await usersGraph.CreateAsync(user.Id);
        return Ok(user.Id);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateAsync([FromForm] UpdateUserDto request,
        CancellationToken cancellationToken)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(user => user.Id == request.Id, cancellationToken);
        if (user is null) return NotFound(nameof(User) + $" {request.Id}");
        
        var addFileResult = await fileRepository
            .AddFileAsync(request.File, user.Id, BucketConstants.Avatar, cancellationToken);
        
        return await addFileResult.Match<Task<IActionResult>>(
            async avatar =>
            {
                request.Adapt(user);
                user.Avatar = avatar;
                await context.SaveChangesAsync(cancellationToken);
                return NoContent();
            }, error => Task.FromResult<IActionResult>(error switch
            {
                NotFoundError err => NotFound(err.Message),
                _ => StatusCode(StatusCodes.Status500InternalServerError, "Not Handled Error")
            }));
    }
}