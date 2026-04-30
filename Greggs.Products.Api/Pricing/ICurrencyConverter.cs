namespace Greggs.Products.Api.Pricing;

public interface ICurrencyConverter
{
    decimal Convert(decimal amount, Currency from, Currency to);
}
