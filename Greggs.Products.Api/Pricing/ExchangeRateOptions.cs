using System.Collections.Generic;

namespace Greggs.Products.Api.Pricing;

public class ExchangeRateOptions
{
    public const string SectionName = "ExchangeRates";

    public Dictionary<string, decimal> Rates { get; set; } = new();
}
