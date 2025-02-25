using Api.Dto;
using Api.Services;
using Domain.Abstractions;
using Domain.Models;
using Infrastructure.Repositories;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController(
    UserManager<User> userManager,
    UserRepository userGraph,
    TokenService tokenService) 
    : ControllerBase
{
    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto request)
    {
        var userExists = await userManager.FindByNameAsync(request.UserName);
        if (userExists is not null)
            return Conflict(Messages.AlreadyExists<User>(request.UserName));

        var user = request.Adapt<User>();
        user.Id = Guid.NewGuid();
        user.SecurityStamp = Guid.NewGuid().ToString();
       
        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded) 
            return BadRequest("User creation failed!");
        
        await userGraph.CreateAsync(user.Id);
        return Ok();
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        var user = await userManager.FindByNameAsync(request.UserName);
        if (user is null)
            return NotFound(Messages.NotFound<User>(request.UserName));

        var isPassCorrect = await userManager.CheckPasswordAsync(user, request.Password);
        if (!isPassCorrect)
            return Unauthorized(Messages.IncorrectPassword);

        var token = tokenService.GenerateJwtToken(user.Id, user.UserName);
        return Ok(token);
    }
}