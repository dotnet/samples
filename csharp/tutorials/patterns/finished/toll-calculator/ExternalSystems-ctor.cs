// suggest owner refactor to constructor
namespace ConsumerVehicleRegistration2
{
    public class Car
    {
        public Car(int passengers) => Passengers = passengers;

        public required int Passengers { get; set; }
    }
}

namespace CommercialRegistration2
{
    public class DeliveryTruck
    {
        public DeliveryTruck(int grossWeight) => GrossWeight = grossWeight;

        public required int GrossWeight { get; set; }
    }
}

namespace LiveryRegistration2
{
    public class Taxi
    {
        public Taxi(int fares) => Fares = fares;

        public required int Fares { get; set; }
    }

    public class Bus
    {
        public Bus(int riders, int capacity = 90)
        {
            Riders = riders;
            Capacity = capacity;
        }

        public required int Capacity { get; set; }
        public required int Riders { get; set; }
    }
}
