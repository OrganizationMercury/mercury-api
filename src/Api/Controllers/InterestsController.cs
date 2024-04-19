using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class InterestsController(InterestRepository interestsGraph) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        var interests = await interestsGraph.GetAllAsync();
        return Ok(interests);
    }

    [HttpPost]
    public async Task<IActionResult> CreateIfNotExists([FromBody] string name)
    {
        await interestsGraph.EnsureCreatedAsync(name);
        return NoContent();
    }
}