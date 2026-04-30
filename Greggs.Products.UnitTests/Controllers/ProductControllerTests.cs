using System.Collections.Generic;
using System.Linq;
using Greggs.Products.Api.Controllers;
using Greggs.Products.Api.DataAccess;
using Greggs.Products.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Greggs.Products.UnitTests.Controllers;

public class ProductControllerTests
{
    [Fact]
    public void Get_WithDefaultPaging_PassesDefaultsToDataAccess()
    {
        var dataAccess = new RecordingProductDataAccess();
        var controller = NewController(dataAccess);

        controller.Get();

        Assert.Equal(0, dataAccess.LastPageStart);
        Assert.Equal(5, dataAccess.LastPageSize);
    }

    [Fact]
    public void Get_WithCustomPaging_PassesParametersToDataAccess()
    {
        var dataAccess = new RecordingProductDataAccess();
        var controller = NewController(dataAccess);

        controller.Get(pageStart: 2, pageSize: 3);

        Assert.Equal(2, dataAccess.LastPageStart);
        Assert.Equal(3, dataAccess.LastPageSize);
    }

    [Fact]
    public void Get_WhenDataAccessReturnsProducts_ReturnsThemInBody()
    {
        var products = new[]
        {
            new Product { Name = "Sausage Roll", PriceInPounds = 1m },
            new Product { Name = "Steak Bake", PriceInPounds = 1.2m }
        };
        var controller = NewController(new RecordingProductDataAccess(products));

        var result = controller.Get();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var body = Assert.IsAssignableFrom<IEnumerable<Product>>(ok.Value).ToList();
        Assert.Equal(2, body.Count);
        Assert.Equal("Sausage Roll", body[0].Name);
        Assert.Equal(1m, body[0].PriceInPounds);
    }

    [Fact]
    public void Get_WhenDataAccessReturnsEmpty_ReturnsEmpty()
    {
        var controller = NewController(new RecordingProductDataAccess());

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
        var dataAccess = new RecordingProductDataAccess();
        var controller = NewController(dataAccess);

        var result = controller.Get(pageStart, pageSize);

        Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.False(dataAccess.WasCalled);
    }

    private static ProductController NewController(IDataAccess<Product> dataAccess)
        => new(dataAccess, NullLogger<ProductController>.Instance);

    private sealed class RecordingProductDataAccess : IDataAccess<Product>
    {
        private readonly IEnumerable<Product> _products;

        public RecordingProductDataAccess(IEnumerable<Product> products = null)
            => _products = products ?? new List<Product>();

        public bool WasCalled { get; private set; }
        public int? LastPageStart { get; private set; }
        public int? LastPageSize { get; private set; }

        public IEnumerable<Product> List(int? pageStart, int? pageSize)
        {
            WasCalled = true;
            LastPageStart = pageStart;
            LastPageSize = pageSize;
            return _products;
        }
    }
}
