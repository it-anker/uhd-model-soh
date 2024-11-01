using System.Linq;
using Mars.Interfaces.Model;
using SOHModel.Ferry.Model;
using Xunit;

namespace SOHTests.FerryModelTests;

public class FerryModelTests
{
    [Fact]
    public void TestGetOutputPropertiesForFerry()
    {
        EntityType type = new EntityType(typeof(Ferry))
        {
            Mapping = new EntityMapping { OutputKind = OutputKind.FullWithIgnored }
        };

        string[] outputProperties = type.OutputProperties.Select(propertyType => propertyType.Name).ToArray();

        Assert.Contains("Length", outputProperties);
        Assert.Contains("MaxSpeed", outputProperties);
        Assert.Contains("Velocity", outputProperties);
    }
}