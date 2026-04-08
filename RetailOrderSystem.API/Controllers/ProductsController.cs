using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailOrderSystem.API.DTOs;
using RetailOrderSystem.API.Services.Interfaces;

namespace RetailOrderSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? categoryId,
        [FromQuery] int? brandId,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var products = await _productService.GetAllAsync(categoryId, brandId, search, page, pageSize);
        return Ok(products);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        return product == null ? NotFound() : Ok(product);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        var created = await _productService.CreateAsync(dto);
        return Ok(created);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateProductDto dto)
    {
        var updated = await _productService.UpdateAsync(id, dto);
        return Ok(updated);
    }
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _productService.DeleteAsync(id);
        return NoContent();
    }
}