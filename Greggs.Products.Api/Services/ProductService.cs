using System.Collections.Generic;
using System.Linq;
using Greggs.Products.Api.DataAccess;
using Greggs.Products.Api.Models;
using Greggs.Products.Api.Pricing;

namespace Greggs.Products.Api.Services;

public class ProductService : IProductService
{
    private const Currency SourceCurrency = Currency.GBP;

    private readonly IDataAccess<Product> _dataAccess;
    private readonly ICurrencyConverter _converter;

    public ProductService(IDataAccess<Product> dataAccess, ICurrencyConverter converter)
    {
        _dataAccess = dataAccess;
        _converter = converter;
    }

    public IEnumerable<ProductDto> GetProducts(int pageStart, int pageSize, Currency currency)
    {
        return _dataAccess.List(pageStart, pageSize)
            .Select(product => new ProductDto
            {
                Name = product.Name,
                Price = _converter.Convert(product.PriceInPounds, SourceCurrency, currency),
                Currency = currency
            });
    }
}
