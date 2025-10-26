using System.Collections.Generic;
using System.Linq;
using Congestion.Calculator.Models;

namespace Congestion.Calculator.Data.HolidayRepository;

public class HolidayRepository(TaxDbContext db) : IHolidayRepository
{
    public List<Holiday> GetAllForYear(int year)
        => db.Holidays.Where(h => h.Date.Year == year).ToList();
}