using Congestion.Calculator.Enums;

namespace Congestion.Calculator.Models;

public class Vehicle : IVehicle
{
    public string PlateNumber { get; set; }
    public VehicleType Type { get; set; }

    public bool IsTaxExempt() => Type switch
    {
        VehicleType.Emergency => true,
        VehicleType.Bus => true,
        VehicleType.Diplomat => true,
        VehicleType.Motorcycle => true,
        VehicleType.Military => true,
        VehicleType.Foreign => true,
        _ => false
    };
}