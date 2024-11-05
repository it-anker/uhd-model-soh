using System.Runtime.Serialization;
using Microsoft.Extensions.DependencyInjection;
using SOH.Process.Server.Persistence;

namespace SOH.Process.Server.Tests;

public class RedisManagementTests : AbstractManagementTests
{
    private readonly IPersistence _persistence;

    public RedisManagementTests(OgcIntegration services) : base(services)
    {
        _persistence = Services.GetRequiredService<IPersistence>();
    }

    [Fact]
    public async Task TestManage()
    {
        TestEntity entity = new();
        await _persistence.UpsertAsync(entity.Id, entity);

        TestEntity? existingEntity = await _persistence.FindAsync<TestEntity>(entity.Id);
        Assert.NotNull(existingEntity);

        TestEntity? deletedEntity = await _persistence.DeleteAsync<TestEntity>(entity.Id);
        Assert.NotNull(deletedEntity);

        TestEntity? deletedAgain = await _persistence.DeleteAsync<TestEntity>(entity.Id);
        Assert.Null(deletedAgain);

        TestEntity? notFound = await _persistence.FindAsync<TestEntity>(entity.Id);
        Assert.Null(notFound);
    }

    public class TestEntity
    {
        [DataMember(Name = "id")] public string Id { get; set; } = Guid.NewGuid().ToString();
    }
}