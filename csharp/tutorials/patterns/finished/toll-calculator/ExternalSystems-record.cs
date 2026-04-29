// suggest owner refactor from class to positional record
namespace ConsumerVehicleRegistration4
{
    public record Car(int Passengers);
}

namespace CommercialRegistration4
{
    public record DeliveryTruck(int GrossWeight);
}

namespace LiveryRegistration4
{
    public record Taxi(int Fares);

    public record Bus(int Riders, int Capacity = 90);
}
