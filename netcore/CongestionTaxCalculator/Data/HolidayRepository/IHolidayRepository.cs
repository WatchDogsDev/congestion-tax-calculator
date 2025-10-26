using System.Collections.Generic;
using Congestion.Calculator.Models;

namespace Congestion.Calculator.Data.HolidayRepository;

public interface IHolidayRepository
{
    List<Holiday> GetAllForYear(int year);
}