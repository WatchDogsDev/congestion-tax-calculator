using System;
using System.Collections.Generic;
using System.Linq;
using Congestion.Calculator.Enums;
using Congestion.Calculator.Models;
using Congestion.Calculator.Types;

namespace Congestion.Calculator.Services.TaxCalculatorService;

public class TaxCalculator(IEnumerable<TaxRule> taxRules, DayRuleChecker dayChecker)
    : ITaxCalculator
{
    public Money CalculateTax(IVehicle vehicle, IEnumerable<Passage> passages)
    {
        const Currency defaultCurrency = Currency.SEK;
        const int maxFeePerDay = 60;

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
            SingleChargeRuleProcessor singleChargeRuleProcessor = new();
            IEnumerable<Passage> processedPassages = singleChargeRuleProcessor.ApplyRule(passagesDayGroup, taxRules);
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
}