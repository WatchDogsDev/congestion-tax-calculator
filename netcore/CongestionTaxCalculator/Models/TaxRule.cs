using System;

namespace Congestion.Calculator.Models;

public class TaxRule
{
    public string CityName { get; set; }
    public TimeSpan Start { get; set; }
    public TimeSpan End { get; set; }
    public int Amount { get; set; }
}