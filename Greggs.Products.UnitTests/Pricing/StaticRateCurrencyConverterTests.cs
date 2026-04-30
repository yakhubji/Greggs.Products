using System;
using System.Collections.Generic;
using Greggs.Products.Api.Pricing;
using Microsoft.Extensions.Options;
using Xunit;

namespace Greggs.Products.UnitTests.Pricing;

public class StaticRateCurrencyConverterTests
{
    [Fact]
    public void Convert_WhenSourceAndTargetMatch_ReturnsAmountUnchanged()
    {
        var converter = NewConverter(("GBP_EUR", 1.11m));

        var result = converter.Convert(2.50m, Currency.GBP, Currency.GBP);

        Assert.Equal(2.50m, result);
    }

    [Fact]
    public void Convert_GbpToEur_AppliesConfiguredRate()
    {
        var converter = NewConverter(("GBP_EUR", 1.11m));

        var result = converter.Convert(1.00m, Currency.GBP, Currency.EUR);

        Assert.Equal(1.11m, result);
    }

    [Theory]
    [InlineData(1.00, 1.11)]
    [InlineData(1.10, 1.22)]
    [InlineData(1.20, 1.33)]
    [InlineData(0.70, 0.78)]
    [InlineData(0.50, 0.56)]
    [InlineData(2.10, 2.33)]
    [InlineData(1.95, 2.16)]
    public void Convert_GbpToEur_RoundsToTwoDecimalPlaces(double amount, double expected)
    {
        var converter = NewConverter(("GBP_EUR", 1.11m));

        var result = converter.Convert((decimal)amount, Currency.GBP, Currency.EUR);

        Assert.Equal((decimal)expected, result);
    }

    [Fact]
    public void Convert_UsesBankersRoundingForMidpointValues()
    {
        // 2.005 * 1 = 2.005 → banker's rounding to 2dp picks the even digit: 2.00
        // (AwayFromZero would give 2.01, biasing totals upward over many sums)
        var converter = NewConverter(("GBP_EUR", 1m));

        var result = converter.Convert(2.005m, Currency.GBP, Currency.EUR);

        Assert.Equal(2.00m, result);
    }

    [Fact]
    public void Convert_WhenRateNotConfigured_Throws()
    {
        var converter = NewConverter();

        var ex = Assert.Throws<InvalidOperationException>(
            () => converter.Convert(1m, Currency.GBP, Currency.EUR));

        Assert.Contains("GBP", ex.Message);
        Assert.Contains("EUR", ex.Message);
    }

    private static StaticRateCurrencyConverter NewConverter(params (string Key, decimal Rate)[] rates)
    {
        var options = new ExchangeRateOptions { Rates = new Dictionary<string, decimal>() };
        foreach (var (key, rate) in rates)
            options.Rates[key] = rate;

        return new StaticRateCurrencyConverter(Options.Create(options));
    }
}
