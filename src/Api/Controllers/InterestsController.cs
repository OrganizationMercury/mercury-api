using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class InterestsController(InterestRepository interestsGraph) : ControllerBase
{
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        var interests = await interestsGraph.GetAllAsync();
        return Ok(interests);
    }

    public async Task<IActionResult> CreateIfNotExists([FromBody]string name)
    {
        await interestsGraph.CreateIfNotExistsAsync(name);
        return NoContent();
    }
}