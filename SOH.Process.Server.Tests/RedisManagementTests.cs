using System.Runtime.Serialization;
using Mars.Common.Core;
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
        NotNull(existingEntity);
        NotNull(existingEntity.Sub);
        Equal(1, existingEntity.Sub.Number);
        Equal("my sub desc", existingEntity.Sub.Description);

        var entities = _persistence
            .ListAsync<TestEntity>("test*")
            .ToBlockingEnumerable()
            .ToList();
        Contains(entities, entity => entity.Id == entity1.Id);

        var deletedEntity = await _persistence.DeleteAsync<TestEntity>(entity1.Id);
        var deletedEntity2 = await _persistence.DeleteAsync<TestEntity>(entity2.Id);
        NotNull(deletedEntity);
        NotNull(deletedEntity2);
        deletedEntity2 = await _persistence.DeleteAsync<TestEntity>(entity2.Id);
        Null(deletedEntity2);
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

        NotNull(loadedEntity);
        NotNull(loadedEntity.FeatureCollection);
        Single(loadedEntity.FeatureCollection);
        var feature = loadedEntity.FeatureCollection[0];
        Equal(new Point(0, 0), feature.Geometry);
        Contains("field", feature.Attributes.GetNames());
        Equal(1, feature.Attributes["field"].Value<int>());
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
        Equal(10, allEntries.Count);
        All(allEntries, entity => StartsWith("test_entity", entity.Id));

        var filter = new ParameterLimit { PageNumber = 1, PageSize = 2 };
        var pageResponse = await _persistence.ListPaginatedAsync<TestEntity>("test_entity*", filter);

        Equal(1, pageResponse.CurrentPage);
        Equal(2, pageResponse.PageSize);
        Equal(2, pageResponse.Data.Count);
        All(pageResponse.Data, entity => StartsWith("test_entity", entity.Id));

        Equal(5, pageResponse.TotalPages);
        True(pageResponse.HasNextPage);
        False(pageResponse.HasPreviousPage);

        filter.PageNumber = 2;
        var nextPage = await _persistence
            .ListPaginatedAsync<TestEntity>("test_entity*", filter);

        All(nextPage.Data, entity =>
        {
            StartsWith("test_entity", entity.Id);
            DoesNotContain(pageResponse.Data, e => e.Id == entity.Id);
        });
        True(nextPage.HasNextPage);
        True(nextPage.HasPreviousPage);

        filter.PageNumber = 5;

        var lastPage = await _persistence
            .ListPaginatedAsync<TestEntity>("test_entity*", filter);

        Equal(5, lastPage.CurrentPage);
        Equal(5, lastPage.TotalPages);
        Equal(10, lastPage.TotalCount);
        Equal(2, lastPage.PageSize);
        Equal(2, lastPage.Data.Count);
        False(lastPage.HasNextPage);
        True(lastPage.HasPreviousPage);
    }

    [Fact]
    public async Task TestManage()
    {
        TestEntity entity = new() { Description = "test_desc", Value = 3.141};
        await _persistence.UpsertAsync(entity.Id, entity);

        var existingEntity = await _persistence.FindAsync<TestEntity>(entity.Id);
        NotNull(existingEntity);
        Equal("test_desc", existingEntity.Description);
        Equal(3.141, existingEntity.Value);

        existingEntity.Description = "updated_desc";
        await _persistence.UpsertAsync(existingEntity.Id, existingEntity);

        var deletedEntity = await _persistence.DeleteAsync<TestEntity>(entity.Id);
        NotNull(deletedEntity);
        Equal("updated_desc", deletedEntity.Description);

        var deletedAgain = await _persistence.DeleteAsync<TestEntity>(entity.Id);
        Null(deletedAgain);

        var notFound = await _persistence.FindAsync<TestEntity>(entity.Id);
        Null(notFound);

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

        [DataMember(Name = "value")]
        public object? Value { get; set; }
    }

    public class SubType
    {
        [DataMember(Name = "description")]
        public string? Description { get; set; }

        [DataMember(Name = "number")]
        public int Number { get; set; } = 1;
    }
}