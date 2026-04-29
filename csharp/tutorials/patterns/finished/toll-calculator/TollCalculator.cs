using System.Diagnostics.CodeAnalysis;
using CommercialRegistration;
using ConsumerVehicleRegistration;
using LiveryRegistration;

namespace Calculators;
public static class TollCalculator
{
    const decimal CarFare = 2.00m, TaxiFare = 3.50m, BusFare = 5.00m, DeliveryTruckFare = 10.00m;

    public static decimal CalculateToll([AllowNull] object vehicle) =>
        vehicle switch
        {
            null => throw new ArgumentNullException(nameof(vehicle)),
            Car c => c.Passengers switch
            {
                0 => CarFare + 0.50m,
                1 => CarFare,
                2 => CarFare - 0.50m,
                _ => CarFare - 1.00m
            },

            Taxi t => t.Fares switch
            {
                0 => TaxiFare + 1.00m,
                1 => TaxiFare,
                2 => TaxiFare - 0.50m,
                _ => TaxiFare - 1.00m
            },

            Bus b => b.Riders / (double)b.Capacity < 0.5d
                ? BusFare + 2.00m
                : b.Riders / (double)b.Capacity > 0.9d
                    ? BusFare - 1.00m
                    : BusFare,

            DeliveryTruck t => t.GrossWeight > 5000
                ? DeliveryTruckFare + 3.50m
                : t.GrossWeight < 3000
                    ? DeliveryTruckFare - 2.00m
                    : DeliveryTruckFare,

            //{ } => throw new ArgumentException(message: "Not a known vehicle type", paramName: nameof(vehicle)),
            _ => throw new ArgumentException(message: "Not a known vehicle type", paramName: nameof(vehicle))
        };

        // <SnippetFinalTuplePattern>
        public static decimal PeakTimePremium(DateTime timeOfToll, bool inbound) =>
            (IsWeekDay(timeOfToll), GetTimeBand(timeOfToll), inbound) switch
            {
                (true, TimeBand.Overnight, _) => 0.75m,
                (true, TimeBand.Daytime, _) => 1.5m,
                (true, TimeBand.MorningRush, true) => 2.0m,
                (true, TimeBand.EveningRush, false) => 2.0m,
                _ => 1.0m
            };
    // </SnippetFinalTuplePattern>

    // <SnippetPremiumWithoutPattern>
    public static decimal PeakTimePremiumIfElse(DateTime timeOfToll, bool inbound)
    {
        if ((timeOfToll.DayOfWeek == DayOfWeek.Saturday) ||
            (timeOfToll.DayOfWeek == DayOfWeek.Sunday))
        {
            return 1.0m;
        }

        int hour = timeOfToll.Hour;
        if (hour < 6) { return 0.75m; }
        if (hour < 10) { return inbound ? 2.0m : 1.0m; }
        if (hour < 16) { return 1.5m; }
        if (hour < 20) { return inbound ? 1.0m : 2.0m; }
        // Overnight
        return 0.75m;
    }
    // </SnippetPremiumWithoutPattern>

    // <SnippetTuplePatternOne>
    public static decimal PeakTimePremiumFull(DateTime timeOfToll, bool inbound) =>
        (IsWeekDay(timeOfToll), GetTimeBand(timeOfToll), inbound) switch
        {
            (true, TimeBand.MorningRush, true) => 2.00m,
            (true, TimeBand.MorningRush, false) => 1.00m,
            (true, TimeBand.Daytime, true) => 1.50m,
            (true, TimeBand.Daytime, false) => 1.50m,
            (true, TimeBand.EveningRush, true) => 1.00m,
            (true, TimeBand.EveningRush, false) => 2.00m,
            (true, TimeBand.Overnight, true) => 0.75m,
            (true, TimeBand.Overnight, false) => 0.75m,
            (false, TimeBand.MorningRush, true) => 1.00m,
            (false, TimeBand.MorningRush, false) => 1.00m,
            (false, TimeBand.Daytime, true) => 1.00m,
            (false, TimeBand.Daytime, false) => 1.00m,
            (false, TimeBand.EveningRush, true) => 1.00m,
            (false, TimeBand.EveningRush, false) => 1.00m,
            (false, TimeBand.Overnight, true) => 1.00m,
            (false, TimeBand.Overnight, false) => 1.00m,
        };
    // </SnippetTuplePatternOne>

    // <SnippetIsWeekDay>
    private static bool IsWeekDay(DateTime timeOfToll) =>
        timeOfToll.DayOfWeek switch
        {
            DayOfWeek.Saturday => false,
            DayOfWeek.Sunday => false,
            _ => true
        };
    // </SnippetIsWeekDay>

    // <SnippetGetTimeBand>
    private enum TimeBand
    {
        MorningRush,
        Daytime,
        EveningRush,
        Overnight
    }

    private static TimeBand GetTimeBand(DateTime timeOfToll) =>
        timeOfToll.Hour switch
        {
            < 6 or >= 20 => TimeBand.Overnight,
            < 10 => TimeBand.MorningRush,
            < 16 => TimeBand.Daytime,
            _ => TimeBand.EveningRush,
        };
    // </SnippetGetTimeBand>
}
