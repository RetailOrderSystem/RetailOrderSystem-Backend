using Microsoft.EntityFrameworkCore;
using RetailOrderSystem.API.Data;
using RetailOrderSystem.API.DTOs;
using RetailOrderSystem.API.Models;
using RetailOrderSystem.API.Services.Interfaces;

namespace RetailOrderSystem.API.Services
{
    public class BrandService : IBrandService
    {
        private readonly AppDbContext _db;

        public BrandService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<BrandDto>> GetAllAsync()
        {
            return await _db.Brands
                .Select(b => new BrandDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    Description = b.Description
                })
                .ToListAsync();
        }

        public async Task<BrandDto?> GetByIdAsync(int id)
        {
            return await _db.Brands
                .Where(b => b.Id == id)
                .Select(b => new BrandDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    Description = b.Description
                })
                .FirstOrDefaultAsync();
        }

        public async Task<BrandDto> CreateAsync(CreateBrandDto dto)
        {
            var brand = new Brand
            {
                Name = dto.Name,
                Description = dto.Description
            };

            _db.Brands.Add(brand);
            await _db.SaveChangesAsync();

            return new BrandDto
            {
                Id = brand.Id,
                Name = brand.Name,
                Description = brand.Description
            };
        }

        public async Task<BrandDto> UpdateAsync(int id, CreateBrandDto dto)
        {
            var brand = await _db.Brands.FindAsync(id)
                ?? throw new InvalidOperationException("Brand not found.");

            brand.Name = dto.Name;
            brand.Description = dto.Description;

            await _db.SaveChangesAsync();

            return new BrandDto
            {
                Id = brand.Id,
                Name = brand.Name,
                Description = brand.Description
            };
        }

        public async Task DeleteAsync(int id)
        {
            var brand = await _db.Brands.FindAsync(id)
                ?? throw new InvalidOperationException("Brand not found.");

            _db.Brands.Remove(brand);
            await _db.SaveChangesAsync();
        }
    }
}
