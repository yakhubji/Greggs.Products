using System.Collections.Generic;
using Greggs.Products.Api.DataAccess;
using Greggs.Products.Api.Models;

namespace Greggs.Products.Api.Services;

public class ProductService : IProductService
{
    private readonly IDataAccess<Product> _dataAccess;

    public ProductService(IDataAccess<Product> dataAccess)
    {
        _dataAccess = dataAccess;
    }

    public IEnumerable<Product> GetProducts(int pageStart, int pageSize)
    {
        return _dataAccess.List(pageStart, pageSize);
    }
}
