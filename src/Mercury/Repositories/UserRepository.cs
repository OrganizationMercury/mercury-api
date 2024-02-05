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
            .Create("""
                    (user:User { 
                    id: id,
                    firstname: firstname,
                    lastname: lastname
                    })
                    """)
            .WithParams(new
            {
                id = user.Id,
                firstname = user.Firstname,
                lastname = user.Lastname
            })
            .ExecuteWithoutResultsAsync();
    }

    public async Task<User> GetUserAsync(Guid id)
    {
        var users = await _client.Cypher
            .Match("(user:User)")
            .Where("user.id = $id")
            .WithParam("id", new { id })
            .Return(user => user.As<User>())
            .Limit(1)
            .ResultsAsync;

        return users.Single();
    }
}