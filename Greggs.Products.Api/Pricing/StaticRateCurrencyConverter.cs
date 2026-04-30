using System;
using Microsoft.Extensions.Options;

namespace Greggs.Products.Api.Pricing;

public class StaticRateCurrencyConverter : ICurrencyConverter
{
    private const int DecimalPlaces = 2;

    private readonly ExchangeRateOptions _options;

    public StaticRateCurrencyConverter(IOptions<ExchangeRateOptions> options)
    {
        _options = options.Value;
    }

    public decimal Convert(decimal amount, Currency from, Currency to)
    {
        if (from == to)
            return amount;

        var key = $"{from}_{to}";
        if (!_options.Rates.TryGetValue(key, out var rate))
            throw new InvalidOperationException(
                $"No exchange rate configured for conversion {from} -> {to}.");

        // Banker's rounding (MidpointRounding.ToEven) is the default for financial
        // calculations: it minimises statistical bias when many rounded values are
        // summed compared to AwayFromZero, which always inflates totals at .x5.
        return Math.Round(amount * rate, DecimalPlaces, MidpointRounding.ToEven);
    }
}
