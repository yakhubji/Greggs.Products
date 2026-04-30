using System.Collections.Generic;
using Greggs.Products.Api.Models;
using Greggs.Products.Api.Pricing;

namespace Greggs.Products.Api.Services;

public interface IProductService
{
    IEnumerable<ProductDto> GetProducts(int pageStart, int pageSize, Currency currency);
}
