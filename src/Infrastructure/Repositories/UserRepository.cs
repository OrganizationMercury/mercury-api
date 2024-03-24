using Domain.Models;
using Mapster;
using Neo4j.Driver;

namespace Infrastructure.Repositories;

public class UserRepository(IDriver driver)
{
    public async Task<User> GetUserAsync(Guid id)
    {
        await using var session = driver.AsyncSession();
        return await session.ExecuteReadAsync(async runner =>
        {
            var data = await runner.RunAsync(
                """
                MATCH (user: User { Id: $Id })
                RETURN {
                    Id: user.Id
                }
                """, new { Id = id.ToString() });
            var record = await data.SingleAsync();
            return record[0].Adapt<User>();
        });
    }
    
    public async Task CreateUserAsync(Guid id)
    {
        await using var session = driver.AsyncSession();
        await session.ExecuteWriteAsync(async runner =>
            await runner.RunAsync(
                """
                CREATE (user:User {
                    Id: $Id
                })
                """, new
                {
                    Id = id.ToString()
                }));
    }
}