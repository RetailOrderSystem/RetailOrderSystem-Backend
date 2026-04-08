using RetailOrderSystem.API.DTOs;

namespace RetailOrderSystem.API.Services.Interfaces
{
    public interface IBrandService
    {
        Task<List<BrandDto>> GetAllAsync();
        Task<BrandDto?> GetByIdAsync(int id);
        Task<BrandDto> CreateAsync(CreateBrandDto dto);
        Task<BrandDto> UpdateAsync(int id, CreateBrandDto dto);
        Task DeleteAsync(int id);
    }
}