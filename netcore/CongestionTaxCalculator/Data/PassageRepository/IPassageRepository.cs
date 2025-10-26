using System.Collections.Generic;
using Congestion.Calculator.Models;

namespace Congestion.Calculator.Data.PassageRepository;

public interface IPassageRepository
{
    void Add(Passage passage);
    void AddRange(IEnumerable<Passage> passages);
    List<Passage> GetListByVehicle(string plateNumber);
}