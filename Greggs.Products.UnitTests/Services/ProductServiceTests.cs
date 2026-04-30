using System.Collections.Generic;
using System.Linq;
using Greggs.Products.Api.DataAccess;
using Greggs.Products.Api.Models;
using Greggs.Products.Api.Services;
using Xunit;

namespace Greggs.Products.UnitTests.Services;

public class ProductServiceTests
{
    [Fact]
    public void GetProducts_ForwardsPagingArgsToDataAccess()
    {
        var dataAccess = new RecordingProductDataAccess();
        var service = new ProductService(dataAccess);

        service.GetProducts(pageStart: 4, pageSize: 7);

        Assert.Equal(4, dataAccess.LastPageStart);
        Assert.Equal(7, dataAccess.LastPageSize);
    }

    [Fact]
    public void GetProducts_ReturnsWhateverDataAccessReturns()
    {
        var products = new[]
        {
            new Product { Name = "Sausage Roll", PriceInPounds = 1m }
        };
        var service = new ProductService(new RecordingProductDataAccess(products));

        var result = service.GetProducts(0, 5).ToList();

        Assert.Single(result);
        Assert.Equal("Sausage Roll", result[0].Name);
    }

    private sealed class RecordingProductDataAccess : IDataAccess<Product>
    {
        private readonly IEnumerable<Product> _products;

        public RecordingProductDataAccess(IEnumerable<Product> products = null)
            => _products = products ?? new List<Product>();

        public int? LastPageStart { get; private set; }
        public int? LastPageSize { get; private set; }

        public IEnumerable<Product> List(int? pageStart, int? pageSize)
        {
            LastPageStart = pageStart;
            LastPageSize = pageSize;
            return _products;
        }
    }
}
