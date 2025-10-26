using System.Linq;
using Congestion.Calculator.Models;

namespace Congestion.Calculator.Data.VehicleRepository;

public class VehicleRepository(TaxDbContext db) : IVehicleRepository
{
    public Vehicle GetOneByPlateNumber(string plateNumber)
        => db.Vehicles.FirstOrDefault(v => v.PlateNumber == plateNumber);

    public void Add(Vehicle vehicle)
    {
        db.Vehicles.Add(vehicle);
        db.SaveChanges();
    }
}

