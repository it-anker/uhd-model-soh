using Mars.Interfaces.Data;
using Mars.Interfaces.Layers;
using NetTopologySuite.Geometries;
using Position = Mars.Interfaces.Environments.Position;

namespace SOHModel.Multimodal.Routing;

/// <summary>
///     Capsules a position represents an exit point of an environment to a distance point.
/// </summary>
public class GatewayPoint : IVectorFeature
{
    public Position Position { get; private set; }

    /// <summary>
    ///     Gets or sets the concrete feature data.
    /// </summary>
    public VectorStructuredData VectorStructured { get; set; }

    public void Init(ILayer layer, VectorStructuredData data)
    {
        Update(data);
    }

    public void Update(VectorStructuredData data)
    {
        VectorStructured = data;
        Point? centroid = VectorStructured.Geometry.Centroid;
        Position = Position.CreateGeoPosition(centroid.X, centroid.Y);
    }
}