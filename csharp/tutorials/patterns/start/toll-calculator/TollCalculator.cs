using CommercialRegistration;
using ConsumerVehicleRegistration;
using LiveryRegistration;

namespace Calculators;

public static class TollCalculator
{
    const decimal CarFare = 2.00m, TaxiFare = 3.50m, BusFare = 5.00m, DeliveryTruckFare = 10.00m;

    /// <summary>
    /// simplest pure function as placeholder to make the "First set of test code" compile
    /// </summary>
    /// <param name="vehicle">specify subject vehicle instance</param>
    /// <returns>decimal toll fare based on conditions YOU will write (hint: see below)</returns>
    /// <exception cref="NotImplementedException"></exception>
    public static decimal CalculateToll(object vehicle) =>
        throw new NotImplementedException("no free lunch here");

/*
    /// <summary>
    /// simple pure function to return toll based on crude vehicle type
    /// </summary>
    /// <param name="vehicle">specify subject vehicle instance</param>
    /// <returns>decimal toll fare based on primitive vehicle type</returns>
    /// <remarks>so far c,t,b,t could be _ discard, but keep reading the docs!</remarks>
    /// <exception cref="ArgumentException"></exception>
    public static decimal CalculateToll(object vehicle) =>
        vehicle switch
        {
            Car c => CarFare,
            Taxi t => TaxiFare,
            Bus b => BusFare,
            DeliveryTruck t => DeliveryTruckFare,
            _ => throw new ArgumentException(message: "Not a known vehicle type", paramName: nameof(vehicle)),
        };
*/
}
