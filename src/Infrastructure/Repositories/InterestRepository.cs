using Domain.Models;
using Mapster;
using Neo4j.Driver;

namespace Infrastructure.Repositories;

public class InterestRepository(IDriver driver)
{
    public async Task<List<Interest>> GetAllAsync(string name)
    {
        await using var session = driver.AsyncSession();
        return await session.ExecuteReadAsync(async runner =>
        {
            var data = await runner.RunAsync(
                """
                MATCH (interest: Interest)
                RETURN { Name: interest.Name }
                """, new { Name = name });
            var records = await data.ToListAsync();
            return records.Select(record => record[0].Adapt<Interest>()).ToList();
        });
    }

    public async Task CreateIfNotExistsAsync(string name)
    {
        await using var session = driver.AsyncSession();
        await session.ExecuteWriteAsync(async runner =>
        {
            var data = await runner.RunAsync(
                """
                MERGE (interest: Interest { Name: $Name }
                RETURN { Name: interest.Name }
                """, new { Name = name });
            return data.ConsumeAsync();
        });

    }
}