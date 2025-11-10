using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Interfaces;

namespace ProductCatalog.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository productRepository)
    {
        _repository = productRepository;
    }
    
    //Funcion privada de mapeo
    private ProductDto MapProductToDto(Product product)
    {
        return new ProductDto
        {
            Name = product.Name,
            Description = product.Description,
            Quantity = product.Quantity,
            Price = product.Price
        };
    }

    public async Task<ProductDto> CreateAsync(ProductDto createDto)
    {
        try
        {
            var product = new Product
            {
                Name = createDto.Name,
                Description = createDto.Description,
                Quantity = createDto.Quantity,
                Price = createDto.Price
            };
            var newProduct = await _repository.AddProduct(product);
            return MapProductToDto(newProduct);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Ocurrio un error creando el producto en el servicio: {e}");
            return null;
        }
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        try
        {
            var product = await _repository.GetAllProducts();
            return product.Select(MapProductToDto).ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Ocurrio un error trayendo todos los productos en el servicio: {e}");
            throw;
        }
    }

    public async Task<ProductDto> GetByIdAsync(int id)
    {
        try
        {
            var product = await _repository.GetProductById(id);
            if (product != null)
            {
                return MapProductToDto(product);
            }

            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Ocurrio un error inesperado trayendo el producto");
            return null;
        }
    }

    public async Task UpdateAsync(int id, ProductDto updateDto)
    {
        try
        {
            var product = await _repository.GetProductById(id);
            if (product != null)
            {
                product.Name = updateDto.Name;
                product.Description = updateDto.Description;
                product.Quantity = updateDto.Quantity;
                product.Price = updateDto.Price;

                await _repository.UpdateProduct(product);
            }
            Console.WriteLine($"El producto que se intenta actualizar es nulo");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Ocurrio un error inesperado actualizando el producto");
        }
    }

    public async Task DeleteAsync(int id)
    {
        try
        {
            var deletedProduct = await _repository.DeleteProduct(id);
            if (deletedProduct == null)
            {
                Console.WriteLine($"El producto con ID: {id} no se pudo encontrar");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Ocurrio un error inesperado elimininando el producto con ID: {id}\n{e}");
        }
    }
}