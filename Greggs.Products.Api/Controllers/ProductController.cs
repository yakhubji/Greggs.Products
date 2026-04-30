using System.Collections.Generic;
using Greggs.Products.Api.DataAccess;
using Greggs.Products.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Greggs.Products.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private const int MaxPageSize = 100;

    private readonly IDataAccess<Product> _productDataAccess;
    private readonly ILogger<ProductController> _logger;

    public ProductController(IDataAccess<Product> productDataAccess, ILogger<ProductController> logger)
    {
        _productDataAccess = productDataAccess;
        _logger = logger;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Product>> Get([FromQuery] int pageStart = 0, [FromQuery] int pageSize = 5)
    {
        if (pageStart < 0 || pageSize <= 0 || pageSize > MaxPageSize)
            return BadRequest("Invalid paging parameters.");

        return Ok(_productDataAccess.List(pageStart, pageSize));
    }
}
