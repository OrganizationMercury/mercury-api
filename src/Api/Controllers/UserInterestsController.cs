using Api.Dto;
using Domain.Abstractions;
using Domain.Models;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("Users/{userId:guid}/Interests")]
public class UserInterestsController(
    InterestRepository interestsGraph,
    UserRepository userGraph, 
    AppDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromRoute] Guid userId)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if(user is null) return NotFound(Messages.NotFound(nameof(User), userId));
        
        var interests = await userGraph.GetUserInterestsAsync(userId);
        return Ok(interests);
    }

    [HttpPost]
    public async Task<IActionResult> LinkAsync(
        [FromRoute] Guid userId,
        [FromBody] InterestDto request)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if(user is null) return NotFound(Messages.NotFound(nameof(User), userId));
        
        await interestsGraph.EnsureCreatedAsync(request.name);
        await userGraph.AddInterestAsync(userId, request.name);
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> UnlinkAsync(
        [FromRoute] Guid userId,
        [FromQuery] InterestDto request)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if(user is null) return NotFound(Messages.NotFound(nameof(User), userId));
        
        var interestExists = await userGraph.UserInterestExistsAsync(userId, request.name);
        if (!interestExists) return NotFound(Messages.NotFound(nameof(Interest), request.name));

        await userGraph.RemoveInterestAsync(userId, request.name);
        return NoContent();
    }
}