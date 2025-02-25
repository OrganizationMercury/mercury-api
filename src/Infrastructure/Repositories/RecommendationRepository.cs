using Neo4j.Driver;

namespace Infrastructure.Repositories;

public class RecommendationRepository(IDriver driver)
{
    public async Task<Guid> RecommendAsync(Guid userId, int index)
    {
        await using var session = driver.AsyncSession();
        return await session.ExecuteReadAsync(async runner =>
        {
            var data = await runner.RunAsync(
                """
                MATCH (currentUser:User { Id: $userId })
                MATCH (currentUser)-[:LIKES]->(interests:Interest)
                MATCH (otherUsers:User)-[:LIKES]->(interests)
                WHERE otherUsers <> currentUser
                WITH otherUsers, COUNT(interests) AS commonInterests
                ORDER BY commonInterests DESC, otherUsers.Id
                RETURN otherUsers.Id AS userId
                SKIP $index
                LIMIT 1
                """, new
                {
                    userId = userId.ToString(),
                    index
                });
            if (!data.IsOpen) throw new ArgumentException("Index out of range for available recommendations.");
            return await data.SingleAsync(record => Guid.Parse(record["userId"].As<string>()));
        });
    }
}