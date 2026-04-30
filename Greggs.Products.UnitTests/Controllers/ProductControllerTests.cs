using System.Collections.Generic;
using System.Linq;
using Greggs.Products.Api.Controllers;
using Greggs.Products.Api.Models;
using Greggs.Products.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Greggs.Products.UnitTests.Controllers;

public class ProductControllerTests
{
    [Fact]
    public void Get_WithDefaultPaging_PassesDefaultsToService()
    {
        var service = new RecordingProductService();
        var controller = NewController(service);

        controller.Get();

        Assert.Equal(0, service.LastPageStart);
        Assert.Equal(5, service.LastPageSize);
    }

    [Fact]
    public void Get_WithCustomPaging_PassesParametersToService()
    {
        var service = new RecordingProductService();
        var controller = NewController(service);

        controller.Get(pageStart: 2, pageSize: 3);

        Assert.Equal(2, service.LastPageStart);
        Assert.Equal(3, service.LastPageSize);
    }

    [Fact]
    public void Get_WhenServiceReturnsProducts_ReturnsThemInBody()
    {
        var products = new[]
        {
            new Product { Name = "Sausage Roll", PriceInPounds = 1m },
            new Product { Name = "Steak Bake", PriceInPounds = 1.2m }
        };
        var controller = NewController(new RecordingProductService(products));

        var result = controller.Get();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var body = Assert.IsAssignableFrom<IEnumerable<Product>>(ok.Value).ToList();
        Assert.Equal(2, body.Count);
        Assert.Equal("Sausage Roll", body[0].Name);
        Assert.Equal(1m, body[0].PriceInPounds);
    }

    [Fact]
    public void Get_WhenServiceReturnsEmpty_ReturnsEmpty()
    {
        var controller = NewController(new RecordingProductService());

        var result = controller.Get();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var body = Assert.IsAssignableFrom<IEnumerable<Product>>(ok.Value);
        Assert.Empty(body);
    }

    [Theory]
    [InlineData(-1, 5)]
    [InlineData(0, 0)]
    [InlineData(0, -1)]
    [InlineData(0, 101)]
    public void Get_WithInvalidPaging_ReturnsBadRequest(int pageStart, int pageSize)
    {
        var service = new RecordingProductService();
        var controller = NewController(service);

        var result = controller.Get(pageStart, pageSize);

        Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.False(service.WasCalled);
    }

    private static ProductController NewController(IProductService service)
        => new(service, NullLogger<ProductController>.Instance);

    private sealed class RecordingProductService : IProductService
    {
        private readonly IEnumerable<Product> _products;

        public RecordingProductService(IEnumerable<Product> products = null)
            => _products = products ?? new List<Product>();

        public bool WasCalled { get; private set; }
        public int LastPageStart { get; private set; }
        public int LastPageSize { get; private set; }

        public IEnumerable<Product> GetProducts(int pageStart, int pageSize)
        {
            WasCalled = true;
            LastPageStart = pageStart;
            LastPageSize = pageSize;
            return _products;
        }
    }
}
