using System.Collections.Generic;
using Congestion.Calculator.Models;
using Congestion.Calculator.Types;

namespace Congestion.Calculator.Services.TaxCalculatorService;

public interface ITaxCalculator
{
    /// <summary>
    /// Calculate the total toll fee for one day
    /// </summary>
    /// <param name="vehicle">The vehicle</param>
    /// <param name="passages">Vehicle passages</param>
    /// <returns>The total congestion tax for that day</returns>
    public Money CalculateTax(IVehicle vehicle, IEnumerable<Passage> passages);
}