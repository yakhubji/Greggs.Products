using System.Collections.Generic;
using Greggs.Products.Api.Models;
using Greggs.Products.Api.Pricing;
using Greggs.Products.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Greggs.Products.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private const int MaxPageSize = 100;

    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<ProductDto>> Get(
        [FromQuery] int pageStart = 0,
        [FromQuery] int pageSize = 5,
        [FromQuery] Currency currency = Currency.GBP)
    {
        if (pageStart < 0 || pageSize <= 0 || pageSize > MaxPageSize)
            return BadRequest("Invalid paging parameters.");

        return Ok(_productService.GetProducts(pageStart, pageSize, currency));
    }
}
