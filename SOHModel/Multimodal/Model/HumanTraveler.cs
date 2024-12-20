using Mars.Common.Core.Random;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Environments;
using SOHModel.Bicycle.Parking;

namespace SOHModel.Multimodal.Model;

/// <summary>
///     This <see cref="Traveler{HumanTravelerLayer}" /> entity uses a variety of modalities to reach its goal.
/// </summary>
public class HumanTraveler : Traveler<HumanTravelerLayer>
{
    private ISet<ModalChoice> _choices = new HashSet<ModalChoice>();

    [PropertyDescription(Name = "hasBike")]
    public double HasBike { get; set; }

    [PropertyDescription(Name = "hasCar")] public double HasCar { get; set; }

    [PropertyDescription(Name = "prefersBike")]
    public double PrefersBike { get; set; }

    [PropertyDescription(Name = "prefersCar")]
    public double PrefersCar { get; set; }

    [PropertyDescription(Name = "usesBikeAndRide")]
    public double UsesBikeAndRide { get; set; }

    [PropertyDescription(Name = "usesOwnBikeOutside")]
    public double UsesOwnBikeOutside { get; set; }

    [PropertyDescription(Name = "usesOwnCar")]
    public double UsesOwnCar { get; set; }

    [PropertyDescription]
    public IBicycleParkingLayer BicycleParkingLayer { get; set; } = default!;

    public override void Init(HumanTravelerLayer layer)
    {
        base.Init(layer);

        Gender = (GenderType)RandomHelper.Random.Next(0, 2);
        OvertakingActivated = false;

        _choices = new ModalityChooser().Evaluate(this);
        _choices.Add(ModalChoice.Walking);

        const int radiusInM = 100;
        if (_choices.Contains(ModalChoice.CyclingOwnBike))
            Bicycle = BicycleParkingLayer.CreateOwnBicycleNear(StartPosition, radiusInM, UsesBikeAndRide);

        if (_choices.Contains(ModalChoice.CarDriving))
            Car = CarParkingLayer.CreateOwnCarNear(StartPosition, radiusInM);
    }

    protected override IEnumerable<ModalChoice> ModalChoices()
    {
        return _choices;
    }
}