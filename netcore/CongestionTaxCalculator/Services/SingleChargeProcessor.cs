using System;
using System.Collections.Generic;
using System.Linq;
using Congestion.Calculator.Models;

namespace Congestion.Calculator.Services;

public class SingleChargeRuleProcessor
{
    public IEnumerable<Passage> ApplyRule(IEnumerable<Passage> passages, IEnumerable<TaxRule> taxRules)
    {
        List<Passage> ordered = passages.OrderBy(p => p.Timestamp).ToList();
        List<Passage> result = [];

        int i = 0;
        while (i < ordered.Count)
        {
            DateTime windowStart = ordered[i].Timestamp;
            DateTime windowEnd = windowStart.AddMinutes(60);

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