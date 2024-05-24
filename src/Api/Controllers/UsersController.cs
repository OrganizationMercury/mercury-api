using Api.Dto;
using Api.Services;
using Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController(UserService users) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        var usersList = await users.GetAllAsync(cancellationToken);
        return Ok(usersList);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, 
        CancellationToken cancellationToken)
    {
        var user = await users.GetByIdAsync(id, cancellationToken);
        if (user is null) return NotFound(Messages.NotFound(nameof(user), id));
        
        return Ok(user);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateAsync([FromForm] UpdateUserDto request,
        CancellationToken cancellationToken)
    {
        await users.UpdateAsync(request, cancellationToken);
        return NoContent();
    }
}