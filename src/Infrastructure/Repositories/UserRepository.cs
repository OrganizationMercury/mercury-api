using Domain.Models;
using Mapster;
using Neo4j.Driver;

namespace Infrastructure.Repositories;

public class UserRepository(IDriver driver)
{
    public async Task<User?> GetByIdAsync(Guid userId)
    {
        await using var session = driver.AsyncSession();
        return await session.ExecuteReadAsync(async runner =>
        {
            var data = await runner.RunAsync(
                """
                MATCH (user: User { Id: $Id })
                RETURN user
                """, new { Id = userId.ToString() });
            var record = await data.SingleAsync();
            return record.Adapt<User>();
        });
    }
    
    public async Task<List<Interest>> GetUserInterestsAsync(Guid userId)
    {
        await using var session = driver.AsyncSession();
        return await session.ExecuteReadAsync(async runner =>
        {
            var data = await runner.RunAsync(
                """
                MATCH (user:User { Id: $Id })-[:LIKES]->(interest: Interest)
                RETURN { Name: interest.Name }
                """, new { Id = userId.ToString() });
            var records = await data.ToListAsync();
            return records.Select(record => record[0].Adapt<Interest>()).ToList();
        });
    }
    
    public async Task<bool> UserInterestExistsAsync(Guid userId, string interestName)
    {
        await using var session = driver.AsyncSession();
        return await session.ExecuteReadAsync(async runner =>
        {
            var data = await runner.RunAsync(
                """
                MATCH (interest: Interest { Name: $Name })
                MATCH (user: User { Id: $Id })-[:LIKES]->(interest)
                RETURN count(interest) as count
                """, new
                {
                    Name = interestName,
                    Id = userId.ToString()
                });
            var record = await data.SingleAsync();
            return record["count"].As<int>() > 0;
        });
    }
    
    public async Task CreateAsync(Guid userId)
    {
        await using var session = driver.AsyncSession();
        await session.ExecuteWriteAsync(async runner =>
            await runner.RunAsync(
                """
                CREATE (:User {
                    Id: $Id
                })
                """, new
                {
                    Id = userId.ToString()
                }));
    }

    public async Task AddInterestAsync(Guid userId, string interestName)
    {
        await using var session = driver.AsyncSession();
        await session.ExecuteWriteAsync(async runner =>
            await runner.RunAsync(
                """
                MATCH (user: User { Id: $Id })
                MATCH (interest: Interest { Name: $Name })
                CREATE (user)-[:LIKES]->(interest)
                """, new
                {
                    Id = userId.ToString(),
                    Name = interestName
                }));
    }
    
    public async Task RemoveInterestAsync(Guid userId, string interestName)
    {
        await using var session = driver.AsyncSession();
        await session.ExecuteWriteAsync(async runner =>
            await runner.RunAsync(
                """
                MATCH (user: User { Id: $Id })
                MATCH (interest: Interest { Name: $Name })
                MATCH (user)-[relation:LIKES]->(interest)
                DELETE relation
                """, new
                {
                    Id = userId.ToString(),
                    Name = interestName
                }));
    }
}