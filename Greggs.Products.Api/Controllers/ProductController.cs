using System.Collections.Generic;
using Greggs.Products.Api.Models;
using Greggs.Products.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Greggs.Products.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private const int MaxPageSize = 100;

    private readonly IProductService _productService;
    private readonly ILogger<ProductController> _logger;

    public ProductController(IProductService productService, ILogger<ProductController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Product>> Get([FromQuery] int pageStart = 0, [FromQuery] int pageSize = 5)
    {
        if (pageStart < 0 || pageSize <= 0 || pageSize > MaxPageSize)
            return BadRequest("Invalid paging parameters.");

        return Ok(_productService.GetProducts(pageStart, pageSize));
    }
}
