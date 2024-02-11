using Mercury.Models;
using Neo4jClient;

namespace Mercury.Repositories;

public class UserRepository
{
    private readonly IGraphClient _client;

    public UserRepository(IGraphClient client)
    {
        _client = client;
    }

    public async Task CreateUserAsync(User user)
    {
        await _client.Cypher
            .Create("(user:User $user)")
            .WithParam("user", user)
            .ExecuteWithoutResultsAsync();
    }

    public async Task<User> GetUserAsync(Guid id)
    {
        var users = await _client.Cypher
            .Match("(user:User)")
            .Where("user.Id = $Id")
            .WithParam("Id", id)
            .Return(user => user.As<User>())
            .ResultsAsync;

        return users.Single();
    }
}