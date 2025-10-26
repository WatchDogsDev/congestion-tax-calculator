using System;
using System.Collections.Generic;
using System.Linq;
using Congestion.Calculator.Enums;
using Congestion.Calculator.Models;
using Congestion.Calculator.Services.DayRuleCheckerService;
using Congestion.Calculator.Types;

namespace Congestion.Calculator.Services.TaxCalculatorService;

public class TaxCalculator(
    City city,
    IEnumerable<TaxRule> taxRules,
    IDayRuleChecker dayChecker)
    : ITaxCalculator
{
    public Money CalculateTax(IVehicle vehicle, IEnumerable<Passage> passages)
    {
        Currency defaultCurrency = city.MoneyCurrency;
        int maxFeePerDay = city.DailyFeeCap;

        if (vehicle.IsTaxExempt())
        {
            return new Money(0, defaultCurrency);
        }

        IEnumerable<IGrouping<DateTime, Passage>> dailyGroups = passages
            .Where(p => dayChecker.IsTaxableDay(p.Timestamp))
            .GroupBy(p => p.Timestamp.Date);

        int totalTax = 0;
        foreach (IGrouping<DateTime, Passage> passagesDayGroup in dailyGroups)
        {
            IEnumerable<Passage> processedPassages = city.HasSingleChargeRule
                ? ApplySingleChargeRule(passagesDayGroup, taxRules, city.SingleChargeRuleWindow!.Value)
                : passagesDayGroup;
            int dayTax = processedPassages.Sum(p =>
            {
                TaxRule rule = taxRules.FirstOrDefault(r =>
                    r.Start <= p.Timestamp.TimeOfDay && r.End > p.Timestamp.TimeOfDay);
                return rule?.Amount ?? 0;
            });

            totalTax += Math.Min(dayTax, maxFeePerDay);
        }

        return new Money(totalTax, defaultCurrency);
    }

    private static IEnumerable<Passage> ApplySingleChargeRule(IEnumerable<Passage> passages,
        IEnumerable<TaxRule> taxRules, TimeSpan timeWindow)
    {
        List<Passage> ordered = passages.OrderBy(p => p.Timestamp).ToList();
        List<Passage> result = [];

        int i = 0;
        while (i < ordered.Count)
        {
            DateTime windowStart = ordered[i].Timestamp;
            DateTime windowEnd = windowStart.Add(timeWindow);

            List<Passage> windowPassages =
                ordered.Where(p => p.Timestamp >= windowStart && p.Timestamp <= windowEnd).ToList();

            if (windowPassages.Count == 0)
            {
                i++;
                continue;
            }

            Passage passageWithHighestTax = windowPassages
                .OrderByDescending(p =>
                    taxRules.FirstOrDefault(r => r.Start <= p.Timestamp.TimeOfDay && r.End > p.Timestamp.TimeOfDay)
                        ?.Amount ?? 0)
                .First();

            result.Add(passageWithHighestTax);
            i = ordered.IndexOf(windowPassages.Last()) + 1;
        }

        return result;
    }
}