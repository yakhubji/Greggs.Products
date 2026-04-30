using System.Collections.Generic;
using System.Linq;
using Greggs.Products.Api.DataAccess;
using Greggs.Products.Api.Models;
using Greggs.Products.Api.Pricing;
using Greggs.Products.Api.Services;
using Xunit;

namespace Greggs.Products.UnitTests.Services;

public class ProductServiceTests
{
    [Fact]
    public void GetProducts_ReturnsOneDtoPerProduct()
    {
        var dataAccess = new StubDataAccess(
            new Product { Name = "Sausage Roll", PriceInPounds = 1m },
            new Product { Name = "Steak Bake", PriceInPounds = 1.2m });
        var service = new ProductService(dataAccess, new IdentityConverter());

        var result = service.GetProducts(0, 5, Currency.GBP).ToList();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GetProducts_DtoCarriesProductName()
    {
        var dataAccess = new StubDataAccess(new Product { Name = "Sausage Roll", PriceInPounds = 1m });
        var service = new ProductService(dataAccess, new IdentityConverter());

        var result = service.GetProducts(0, 5, Currency.GBP).Single();

        Assert.Equal("Sausage Roll", result.Name);
    }

    [Fact]
    public void GetProducts_PriceComesFromConverter()
    {
        var dataAccess = new StubDataAccess(new Product { Name = "Sausage Roll", PriceInPounds = 1m });
        var converter = new RecordingConverter(returnedAmount: 1.11m);
        var service = new ProductService(dataAccess, converter);

        var result = service.GetProducts(0, 5, Currency.EUR).Single();

        Assert.Equal(1.11m, result.Price);
    }

    [Fact]
    public void GetProducts_AlwaysAsksConverterToConvertFromGbp()
    {
        var dataAccess = new StubDataAccess(new Product { Name = "Sausage Roll", PriceInPounds = 1m });
        var converter = new RecordingConverter();
        var service = new ProductService(dataAccess, converter);

        service.GetProducts(0, 5, Currency.EUR).ToList();

        Assert.Equal(Currency.GBP, converter.LastFrom);
    }

    [Fact]
    public void GetProducts_AsksConverterToConvertToRequestedCurrency()
    {
        var dataAccess = new StubDataAccess(new Product { Name = "Sausage Roll", PriceInPounds = 1m });
        var converter = new RecordingConverter();
        var service = new ProductService(dataAccess, converter);

        service.GetProducts(0, 5, Currency.EUR).ToList();

        Assert.Equal(Currency.EUR, converter.LastTo);
    }

    [Fact]
    public void GetProducts_DtoCurrencyMatchesRequested()
    {
        var dataAccess = new StubDataAccess(new Product { Name = "Sausage Roll", PriceInPounds = 1m });
        var service = new ProductService(dataAccess, new IdentityConverter());

        var result = service.GetProducts(0, 5, Currency.EUR).Single();

        Assert.Equal(Currency.EUR, result.Currency);
    }

    [Fact]
    public void GetProducts_ForwardsPagingArgsToDataAccess()
    {
        var dataAccess = new StubDataAccess();
        var service = new ProductService(dataAccess, new IdentityConverter());

        service.GetProducts(pageStart: 4, pageSize: 7, Currency.GBP).ToList();

        Assert.Equal(4, dataAccess.LastPageStart);
        Assert.Equal(7, dataAccess.LastPageSize);
    }

    [Fact]
    public void GetProducts_NoData_ReturnsEmpty()
    {
        var service = new ProductService(new StubDataAccess(), new IdentityConverter());

        var result = service.GetProducts(0, 5, Currency.GBP);

        Assert.Empty(result);
    }

    private sealed class StubDataAccess : IDataAccess<Product>
    {
        private readonly IEnumerable<Product> _products;

        public StubDataAccess(params Product[] products) => _products = products;

        public int? LastPageStart { get; private set; }
        public int? LastPageSize { get; private set; }

        public IEnumerable<Product> List(int? pageStart, int? pageSize)
        {
            LastPageStart = pageStart;
            LastPageSize = pageSize;
            return _products;
        }
    }

    private sealed class RecordingConverter : ICurrencyConverter
    {
        private readonly decimal _returnedAmount;

        public RecordingConverter(decimal returnedAmount = 0m) => _returnedAmount = returnedAmount;

        public Currency LastFrom { get; private set; }
        public Currency LastTo { get; private set; }

        public decimal Convert(decimal amount, Currency from, Currency to)
        {
            LastFrom = from;
            LastTo = to;
            return _returnedAmount;
        }
    }

    private sealed class IdentityConverter : ICurrencyConverter
    {
        public decimal Convert(decimal amount, Currency from, Currency to) => amount;
    }
}
