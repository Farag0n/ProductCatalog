using ProductCatalog.Application.DTOs;

namespace ProductCatalog.Application.Interfaces;

public interface IProductService
{
    Task<ProductDto> CreateAsync(ProductDto createDto);
    Task<IEnumerable<ProductDto>> GetAllAsync();
    Task<ProductDto> GetByIdAsync(int id);
    Task UpdateAsync(int id, ProductDto updateDto);
    Task DeleteAsync(int id);
}