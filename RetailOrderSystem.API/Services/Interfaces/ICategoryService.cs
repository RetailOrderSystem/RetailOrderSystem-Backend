using RetailOrderSystem.API.DTOs;

namespace RetailOrderSystem.API.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetAllAsync();
        Task<CategoryDto?> GetByIdAsync(int id);
        Task<CategoryDto> CreateAsync(CreateCategoryDto dto);
        Task<CategoryDto> UpdateAsync(int id, CreateCategoryDto dto);
        Task DeleteAsync(int id);
    }
}