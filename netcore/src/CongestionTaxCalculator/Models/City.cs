using System;
using Congestion.Calculator.Enums;

namespace Congestion.Calculator.Models;

public class City
{
    public string Name { get; set; }
    public int DailyFeeCap { get; set; }
    public Currency MoneyCurrency { get; set; }
    public bool HasSingleChargeRule => SingleChargeRuleWindow != null;
    public TimeSpan? SingleChargeRuleWindow { get; set; }
}