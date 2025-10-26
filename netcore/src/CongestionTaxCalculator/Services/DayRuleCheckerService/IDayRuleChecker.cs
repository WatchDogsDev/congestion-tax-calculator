using System;

namespace Congestion.Calculator.Services.DayRuleCheckerService;

public interface IDayRuleChecker
{
    bool IsTaxableDay(DateTime date);
}