using Mapster;
using Mercury.Models;
using Neo4j.Driver;

namespace Mercury.Repositories;

public class UserRepository
{
    private readonly IDriver _driver;

    public UserRepository(IDriver driver) => _driver = driver;
    
    public async Task<User> GetUserAsync(Guid id)
    {
        await using var session = _driver.AsyncSession();
        return await session.ExecuteReadAsync(async runner =>
        {
            var data = await runner.RunAsync(
                """
                MATCH (user: User { Id: $Id })
                RETURN {
                    Id: user.Id,
                    Firstname: user.Firstname,
                    Lastname: user.Lastname
                }
                """, new { Id = id.ToString() });
            var record = await data.SingleAsync();
            return record[0].Adapt<User>();
        });
    }
    
    public async Task CreateUserAsync(User user)
    {
        await using var session = _driver.AsyncSession();
        await session.ExecuteWriteAsync(async runner =>
            await runner.RunAsync(
                """
                CREATE (user:User {
                    Id: $Id,
                    Firstname: $Firstname,
                    Lastname: $Lastname
                })
                """, new
                {
                    Id = user.Id.ToString(),
                    user.Firstname,
                    user.Lastname
                }));
    }

    public async Task UpdateUserAsync(User user)
    {
        await using var session = _driver.AsyncSession();
        await session.ExecuteWriteAsync(async runner =>
            await runner.RunAsync(
                """
                MATCH (user:User { Id: $Id })
                SET user.Firstname = $Firstname,
                    user.Lastname = $Lastname
                """, new
                {
                    Id = user.Id.ToString(),
                    user.Firstname,
                    user.Lastname
                }));
    }
    
    public async Task DeleteUserAsync(Guid id)
    {
        await using var session = _driver.AsyncSession();
        await session.ExecuteWriteAsync(async runner =>
            await runner.RunAsync(
            """
            MATCH (user:User { Id: $Id })
            DETACH DELETE user
            """, new { Id = id.ToString() }
            ));
    }
}