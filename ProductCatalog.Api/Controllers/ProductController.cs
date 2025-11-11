using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Interfaces;

namespace ProductCatalog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }
    
    // Obtener todos los productos
    [HttpGet]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    //obtener productos por ID
    [HttpGet("{id:int}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var products = await _productService.GetByIdAsync(id);
            return Ok(products);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    //Crear un nuevo producto
    [HttpPost]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> Add([FromBody] ProductDto showDto)
    {
        try
        {
            var newProduct = await _productService.CreateAsync(showDto);
            return CreatedAtAction(nameof(GetById), new { id = newProduct.Id }, newProduct);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    //Actualizar estudiante existente
    [HttpPut("{id:int}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] ProductDto updateDto)
    {
        try
        {
            await _productService.UpdateAsync(id, updateDto);
            return Ok("Producto actualizado correctamente.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    //Eliminar producto
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _productService.DeleteAsync(id);
            return Ok("Producto eliminado correctamente.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}