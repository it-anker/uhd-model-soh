using System.Runtime.Serialization;
using Mars.Common.Core;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using SOH.Process.Server.Models.Processes;
using SOH.Process.Server.Persistence;
using SOH.Process.Server.Tests.Base;
using FeatureCollection = NetTopologySuite.Features.FeatureCollection;

namespace SOH.Process.Server.Tests;

public class RedisManagementTests : AbstractManagementTests
{
    private readonly IPersistence _persistence;

    public RedisManagementTests(OgcIntegration services) : base(services)
    {
        _persistence = Services.GetRequiredService<IPersistence>();
    }

    [Fact]
    public async Task TestSearchForSubProperty()
    {
        TestEntity entity1 = new ()
        {
            Description = "my desc",
            Sub = new SubType
            {
                Number = 1,
                Description = "my sub desc"
            }
        };

        TestEntity entity2 = new ()
        {
            Description = "my other desc",
        };

        await _persistence.UpsertAsync(entity1.Id, entity1);
        await _persistence.UpsertAsync(entity2.Id, entity2);

        var existingEntity = await _persistence.FindAsync<TestEntity>(entity1.Id);
        Assert.NotNull(existingEntity);
        Assert.NotNull(existingEntity.Sub);
        Assert.Equal(1, existingEntity.Sub.Number);
        Assert.Equal("my sub desc", existingEntity.Sub.Description);

        var entities = _persistence
            .ListAsync<TestEntity>("test*")
            .ToBlockingEnumerable()
            .ToList();
        Assert.Contains(entities, entity => entity.Id == entity1.Id);

        var deletedEntity = await _persistence.DeleteAsync<TestEntity>(entity1.Id);
        var deletedEntity2 = await _persistence.DeleteAsync<TestEntity>(entity2.Id);
        Assert.NotNull(deletedEntity);
        Assert.NotNull(deletedEntity2);
        deletedEntity2 = await _persistence.DeleteAsync<TestEntity>(entity2.Id);
        Assert.Null(deletedEntity2);
    }

    [Fact]
    public async Task TestManageGeoJson()
    {
        var collection = new FeatureCollection
        {
            new Feature(new Point(0, 0), new AttributesTable
            {
                { "field", 1 }
            })
        };

        string id = Guid.NewGuid().ToString();
        var entity = new TestEntity
        {
            Description = "my geoJson entity", FeatureCollection = collection, Id = id
        };
        await _persistence.UpsertAsync(entity.Id, entity);

        var loadedEntity = await _persistence.FindAsync<TestEntity>(entity.Id);

        Assert.NotNull(loadedEntity);
        Assert.NotNull(loadedEntity.FeatureCollection);
        Assert.Single(loadedEntity.FeatureCollection);
        var feature = loadedEntity.FeatureCollection[0];
        Assert.Equal(new Point(0, 0), feature.Geometry);
        Assert.Contains("field", feature.Attributes.GetNames());
        Assert.Equal(1, feature.Attributes["field"].Value<int>());
    }

    [Fact]
    public async Task TestSearchPaginated()
    {
        for (int i = 0; i < 10; i++)
        {
            var entity = new TestEntity
            {
                Description = i.ToString(),
                Id = "test_entity" + Guid.NewGuid()
            };
            await _persistence.UpsertAsync(entity.Id, entity);
        }

        var allEntries = _persistence
            .ListAsync<TestEntity>("test_entity*")
            .ToBlockingEnumerable().ToList();
        Assert.Equal(10, allEntries.Count);
        Assert.All(allEntries, entity => Assert.StartsWith("test_entity", entity.Id));

        var filter = new ParameterLimit { PageNumber = 1, PageSize = 2 };
        var pageResponse = await _persistence.ListPaginatedAsync<TestEntity>("test_entity*", filter);

        Assert.Equal(1, pageResponse.CurrentPage);
        Assert.Equal(2, pageResponse.PageSize);
        Assert.Equal(2, pageResponse.Data.Count);
        Assert.All(pageResponse.Data, entity => Assert.StartsWith("test_entity", entity.Id));

        Assert.Equal(5, pageResponse.TotalPages);
        Assert.True(pageResponse.HasNextPage);
        Assert.False(pageResponse.HasPreviousPage);

        filter.PageNumber = 2;
        var nextPage = await _persistence
            .ListPaginatedAsync<TestEntity>("test_entity*", filter);

        Assert.All(nextPage.Data, entity =>
        {
            Assert.StartsWith("test_entity", entity.Id);
            Assert.DoesNotContain(pageResponse.Data, e => e.Id == entity.Id);
        });
        Assert.True(nextPage.HasNextPage);
        Assert.True(nextPage.HasPreviousPage);

        filter.PageNumber = 5;

        var lastPage = await _persistence
            .ListPaginatedAsync<TestEntity>("test_entity*", filter);

        Assert.Equal(5, lastPage.CurrentPage);
        Assert.Equal(5, lastPage.TotalPages);
        Assert.Equal(10, lastPage.TotalCount);
        Assert.Equal(2, lastPage.PageSize);
        Assert.Equal(2, lastPage.Data.Count);
        Assert.False(lastPage.HasNextPage);
        Assert.True(lastPage.HasPreviousPage);
    }

    [Fact]
    public async Task TestManage()
    {
        TestEntity entity = new() { Description = "test_desc" };
        await _persistence.UpsertAsync(entity.Id, entity);

        var existingEntity = await _persistence.FindAsync<TestEntity>(entity.Id);
        Assert.NotNull(existingEntity);
        Assert.Equal("test_desc", existingEntity.Description);

        existingEntity.Description = "updated_desc";
        await _persistence.UpsertAsync(existingEntity.Id, existingEntity);

        var deletedEntity = await _persistence.DeleteAsync<TestEntity>(entity.Id);
        Assert.NotNull(deletedEntity);
        Assert.Equal("updated_desc", deletedEntity.Description);

        var deletedAgain = await _persistence.DeleteAsync<TestEntity>(entity.Id);
        Assert.Null(deletedAgain);

        var notFound = await _persistence.FindAsync<TestEntity>(entity.Id);
        Assert.Null(notFound);

    }

    public class TestEntity
    {
        [DataMember(Name = "id")] public string Id { get; set; } = $"test:{Guid.NewGuid()}";

        [DataMember(Name = "description")]
        public string? Description { get; set; }

        [DataMember(Name = "sub")]
        public SubType? Sub { get; set; }

        [DataMember(Name = "featureCollection")]
        public FeatureCollection? FeatureCollection { get; set; }
    }

    public class SubType
    {
        [DataMember(Name = "description")]
        public string? Description { get; set; }

        [DataMember(Name = "number")]
        public int Number { get; set; } = 1;
    }
}