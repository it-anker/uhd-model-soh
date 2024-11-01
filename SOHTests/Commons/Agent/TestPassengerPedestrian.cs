using Mars.Interfaces.Environments;
using SOHModel.Ferry.Model;
using SOHModel.Ferry.Station;
using SOHModel.Train.Model;
using SOHModel.Train.Station;

namespace SOHTests.Commons.Agent;

/// <summary>
///     Pedestrian that can use other transportation vehicles as passenger
/// </summary>
public class TestPassengerPedestrian : TestMultiCapableAgent
{
    public Ferry UsedFerry { get; private set; }
    public Train UsedTrain { get; private set; }

    protected override bool EnterModalType(ModalChoice modalChoice, Route route)
    {
        if (modalChoice == ModalChoice.Ferry)
        {
            FerryStation ferryStation = FerryStationLayer.Nearest(Position);
            Ferry ferry = ferryStation.Find(route.Goal);
            bool result = TryEnterVehicleAsPassenger(ferry, this);
            if (result)
            {
                UsedFerry = ferry;
                HasUsedFerry = true;
            }

            return result;
        }

        if (modalChoice == ModalChoice.Train)
        {
            TrainStation station = TrainStationLayer.Nearest(Position);
            Train train = station.Find(route.Goal);
            bool result = TryEnterVehicleAsPassenger(train, this);
            if (result)
            {
                UsedTrain = train;
                HasUsedTrain = true;
            }

            return result;
        }

        return base.EnterModalType(modalChoice, route);
    }
}