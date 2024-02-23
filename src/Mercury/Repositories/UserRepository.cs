using Mapster;
using Mercury.Models;
using Neo4j.Driver;

namespace Mercury.Repositories;

public class UserRepository
{
    private readonly IDriver _driver;

    public UserRepository(IDriver driver)
    {
        _driver = driver;
    }

    public async Task CreateUserAsync(User user)
    {
        await using var session = _driver.AsyncSession();
        await session.ExecuteWriteAsync(async runner =>
            await runner.RunAsync(
                """
                CREATE (user:User {
                    Id: $id,
                    Firstname: $firstname,
                    Lastname: $lastname
                })
                """, new
                {
                    id = user.Id.ToString(),
                    firstname = user.Firstname,
                    lastname = user.Lastname
                }));
    }
    
    public async Task<User> GetUserAsync(Guid id)
    {
        await using var session = _driver.AsyncSession();
        return await session.ExecuteReadAsync(async runner =>
        {
            var data = await runner.RunAsync(
                """
                MATCH (user: User { Id: $id })
                RETURN {
                    Id: user.Id,
                    Firstname: user.Firstname,
                    Lastname: user.Lastname
                }
                """, new { id = id.ToString() });
            var record = await data.SingleAsync();
            return record[0].Adapt<User>();
        });
    }
}