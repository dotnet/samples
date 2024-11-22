// suggest owner refactor to primary constructor
namespace ConsumerVehicleRegistration3
{
    public class Car(int passengers)
    {
        public required int Passengers { get; set; } = passengers;
    }
}

namespace CommercialRegistration3
{
    public class DeliveryTruck(int grossWeight)
    {
        public required int GrossWeight { get; set; } = grossWeight;
    }
}

namespace LiveryRegistration3
{
    public class Taxi(int fares)
    {
        public required int Fares { get; set; } = fares;
    }

    public class Bus(int riders, int capacity = 90)
    {
        public required int Capacity { get; set; } = capacity;
        public required int Riders { get; set; } = riders;
    }
}
