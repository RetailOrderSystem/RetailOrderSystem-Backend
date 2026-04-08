using RetailOrderSystem.API.DTOs;

namespace RetailOrderSystem.API.Services.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductDto>> GetAllAsync(
            int? categoryId,
            int? brandId,
            string? search,
            int page,
            int pageSize);

        Task<ProductDto?> GetByIdAsync(int id);

        Task<ProductDto> CreateAsync(CreateProductDto dto);

        Task<ProductDto> UpdateAsync(int id, CreateProductDto dto);

        Task DeleteAsync(int id);
    }
}