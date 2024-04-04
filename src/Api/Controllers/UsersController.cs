using Api.Dto;
using Domain;
using Domain.Models;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using File = Domain.Models.File;

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
        var user = new User 
        { 
            Id = Guid.NewGuid(),
            Firstname = request.Firstname,
            Lastname = request.Lastname,
            Username = request.Username
        };

        await context.Users.AddAsync(user, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        await usersGraph.CreateAsync(user.Id);
        return Ok(user.Id);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateAsync([FromForm] UpdateUserDto request, [FromForm] IFormFile file,
        CancellationToken cancellationToken)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(user => user.Id == request.Id, cancellationToken);
        if (user is null) return NotFound(nameof(User) + $" {request.Id}");

        var fileId = Guid.NewGuid();
        var fileName = $"{fileId}.jpg";
        await fileRepository.AddFile(file, fileName, BucketConstants.Avatar, cancellationToken);
        
        user.Firstname = request.Firstname;
        user.Lastname = request.Lastname;
        user.Username = request.Username;
        user.Bio = request.Bio;
        user.Avatar = new File
        {
            Id = fileId,
            UserId = request.Id,
            Bucket = BucketConstants.Avatar
        };
        await context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}