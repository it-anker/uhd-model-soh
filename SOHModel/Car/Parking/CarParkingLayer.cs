using Mars.Common.Core.Random;
using Mars.Components.Layers;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Data;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;
using SOHModel.Car.Model;
using SOHModel.Domain.Graph;

namespace SOHModel.Car.Parking;

/// <summary>
///     The <code>CarParkingLayer</code> capsules the access to all <code>CarParkingSpace</code>s.
/// </summary>
public class CarParkingLayer : VectorLayer<CarParkingSpace>, ICarParkingLayer
{
    /// <summary>
    ///     Provides access to the <see cref="ISpatialGraphEnvironment" /> on which the <see cref="Car" />s move.
    /// </summary>
    public ISpatialGraphLayer StreetLayer { get; set; } = default!;

    [PropertyDescription(Name = "occupancyProbability")]
    public double OccupancyProbability { get; set; }

    public ModalChoice ModalChoice => ModalChoice.CarDriving;

    public override bool InitLayer(LayerInitData layerInitData, 
        RegisterAgent? registerAgentHandle = null,
        UnregisterAgent? unregisterAgent = null)
    {
        bool initialized = base.InitLayer(layerInitData, registerAgentHandle, unregisterAgent);
        
        if (StreetLayer == null)
            throw new ArgumentException($"{nameof(CarParkingLayer)} requires {nameof(StreetLayer)}");

        if (initialized && OccupancyProbability > 0)
            UpdateOccupancy(OccupancyProbability);

        return initialized;
    }

    public void UpdateOccupancy(double percent, int carCount = 0)
    {
        int spacesCount = Features.Count;
        double percentReservedForCars = 1.0 * (spacesCount - carCount) / spacesCount;
        double percentCombined = percent * percentReservedForCars;

        foreach (CarParkingSpace parking in Features.OfType<CarParkingSpace>())
        {
            parking.Occupied = RandomHelper.Random.NextDouble() < percentCombined;
        }
    }

    public CarParkingSpace? Nearest(Position position, bool freeCapacity = true)
    {
        bool Predicate(CarParkingSpace parkingSpace)
        {
            return !freeCapacity || parkingSpace.HasCapacity;
        }

        return Nearest(position.PositionArray, Predicate);
    }

    public Model.Car CreateOwnCarNear(Position position, double radiusInM = -1, string keyAttribute = "type",
        string type = "Golf")
    {
        static bool Predicate(CarParkingSpace parkingSpace)
        {
            return parkingSpace.HasCapacity;
        }

        CarParkingSpace carParkingSpace;
        if (radiusInM <= 0)
        {
            carParkingSpace = Nearest(position);
        }
        else
        {
            CarParkingSpace? space = Region(position.PositionArray, radiusInM, Predicate).FirstOrDefault();
            carParkingSpace = space ?? Nearest(position);
        }

        if (carParkingSpace != null)
        {
            Model.Car? car = EntityManager.Create<Model.Car>(keyAttribute, type);
            car.Environment = StreetLayer.Environment;
            car.CarParkingLayer = this;
            if (carParkingSpace.Enter(car))
            {
                car.Position = car.CarParkingSpace.Position;
                return car;
            }
        }

        throw new ApplicationException("No free parking space available to create an own car");
    }
}