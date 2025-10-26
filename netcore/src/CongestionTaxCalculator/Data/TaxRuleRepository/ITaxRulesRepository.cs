using System.Collections.Generic;
using Congestion.Calculator.Models;

namespace Congestion.Calculator.Data.TaxRuleRepository;

public interface ITaxRuleRepository
{
    List<TaxRule> GetAllForCity(string cityName);
}