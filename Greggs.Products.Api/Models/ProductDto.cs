using Greggs.Products.Api.Pricing;

namespace Greggs.Products.Api.Models;

public class ProductDto
{
    public string Name { get; init; }
    public decimal Price { get; init; }
    public Currency Currency { get; init; }
}
