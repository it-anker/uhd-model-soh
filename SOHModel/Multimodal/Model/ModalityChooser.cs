using Mars.Common.Core.Random;
using Mars.Interfaces.Environments;

namespace SOHModel.Multimodal.Model;

public class ModalityChooser
{
    public ISet<ModalChoice> Evaluate(HumanTraveler attributes)
    {
        if (RandomHelper.Random.NextDouble() < attributes.HasCar)
            return new HashSet<ModalChoice> { ModalChoice.CarDriving };

        if (RandomHelper.Random.NextDouble() < attributes.HasBike)
            return new HashSet<ModalChoice> { ModalChoice.CyclingOwnBike };

        if (RandomHelper.Random.NextDouble() < attributes.PrefersCar)
            return new HashSet<ModalChoice> { ModalChoice.CarRentalDriving };

        if (RandomHelper.Random.NextDouble() < attributes.PrefersBike)
            return new HashSet<ModalChoice> { ModalChoice.CyclingRentalBike };

        return new HashSet<ModalChoice> { ModalChoice.Walking };
    }
}