using Domain.Models;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class RecommendationsController(RecommendationRepository recommendations, UserManager<User> users) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetByIndex([FromQuery] Guid userId, [FromQuery] int index)
    {
        var recommendedId = await recommendations.RecommendAsync(userId, index);
        var user = await users.FindByIdAsync(recommendedId.ToString());
        return Ok(user);
    }
}