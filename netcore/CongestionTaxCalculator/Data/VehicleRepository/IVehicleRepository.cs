using Congestion.Calculator.Models;

namespace Congestion.Calculator.Data.VehicleRepository;

public interface IVehicleRepository
{
    Vehicle GetOneByPlateNumber(string plateNumber);
    void Add(Vehicle vehicle);
}