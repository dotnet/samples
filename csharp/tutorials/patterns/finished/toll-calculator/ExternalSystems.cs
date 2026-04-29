// you are not owner, so YOU have no control over these external namespaces or definitions
// but OWNER could refactor with constructor, or from class to positional record [examples below]
namespace ConsumerVehicleRegistration
{
    public class Car
    {
        public required int Passengers { get; set; }
    }
}

namespace CommercialRegistration
{
    public class DeliveryTruck
    {
        public int GrossWeight { get; set; } = 4000;    // default weight (hence required omitted)
    }
}

namespace LiveryRegistration
{
    public class Taxi
    {
        public required int Fares { get; set; }
    }

    public class Bus
    {
        public required int Capacity { get; set; }
        public required int Riders { get; set; }
    }
}
