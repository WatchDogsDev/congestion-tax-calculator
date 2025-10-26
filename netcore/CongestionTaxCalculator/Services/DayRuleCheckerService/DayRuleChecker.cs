using System;
using System.Collections.Generic;
using System.Linq;
using Congestion.Calculator.Models;

namespace Congestion.Calculator.Services.DayRuleCheckerService;

public class DayRuleChecker(IEnumerable<Holiday> holidays) : IDayRuleChecker
{
    public bool IsTaxableDay(DateTime date)
    {
        bool isJuly = date.Month == 7;
        if (isJuly)
        {
            return false;
        }

        if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
        {
            return false;
        }

        bool isHoliday = holidays.Any(h => h.Date.Date == date.Date);
        if (isHoliday)
        {
            return false;
        }

        bool isDayBeforeHoliday = holidays.Any(h => h.Date.Date == date.AddDays(1).Date);
        return !isDayBeforeHoliday;
    }
}