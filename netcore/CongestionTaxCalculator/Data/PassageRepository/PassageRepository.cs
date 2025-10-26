using System.Collections.Generic;
using System.Linq;
using Congestion.Calculator.Models;

namespace Congestion.Calculator.Data.PassageRepository;

public class PassageRepository(TaxDbContext db) : IPassageRepository
{
    public void Add(Passage passage)
    {
        db.Passages.Add(passage);
        db.SaveChanges();
    }

    public void AddRange(IEnumerable<Passage> passages)
    {
        db.Passages.AddRange(passages);
        db.SaveChanges();
    }

    public List<Passage> GetListByVehicle(string plateNumber)
        => db.Passages.Where(p => p.VehiclePlate == plateNumber).ToList();
}