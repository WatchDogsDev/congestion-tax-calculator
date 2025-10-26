using System.Collections.Generic;
using System.Linq;
using Congestion.Calculator.Models;

namespace Congestion.Calculator.Data.TaxRuleRepository;

public class TaxRuleRepository(TaxDbContext db) : ITaxRuleRepository
{
    public List<TaxRule> GetAllForCity(string cityName)
    {
        return db.TaxRules.Where(x => x.CityName == cityName).ToList();
    }
}