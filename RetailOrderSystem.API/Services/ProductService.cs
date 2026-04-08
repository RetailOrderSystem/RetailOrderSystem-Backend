using Microsoft.EntityFrameworkCore;
using RetailOrderSystem.API.Data;
using RetailOrderSystem.API.DTOs;
using RetailOrderSystem.API.Models;
using RetailOrderSystem.API.Services.Interfaces;

namespace RetailOrderSystem.API.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _db;

        public ProductService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<ProductDto>> GetAllAsync(
            int? categoryId,
            int? brandId,
            string? search,
            int page,
            int pageSize)
        {
            var query = _db.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Inventory)
                .AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (brandId.HasValue)
                query = query.Where(p => p.BrandId == brandId.Value);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.Name.Contains(search));

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => MapToDto(p))
                .ToListAsync();
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _db.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Inventory)
                .FirstOrDefaultAsync(p => p.Id == id);

            return product == null ? null : MapToDto(product);
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                BrandId = dto.BrandId,
                CategoryId = dto.CategoryId,
                IsAvailable = true
            };

            _db.Products.Add(product);
            await _db.SaveChangesAsync();

            var inventory = new Inventory
            {
                ProductId = product.Id,
                StockQuantity = dto.Stock,
                LastUpdated = DateTime.UtcNow
            };

            _db.Inventories.Add(inventory);
            await _db.SaveChangesAsync();

            var createdProduct = await _db.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Inventory)
                .FirstAsync(p => p.Id == product.Id);

            return MapToDto(createdProduct);
        }

        public async Task<ProductDto> UpdateAsync(int id, CreateProductDto dto)
        {
            var product = await _db.Products
                .Include(p => p.Inventory)
                .FirstOrDefaultAsync(p => p.Id == id)
                ?? throw new InvalidOperationException("Product not found.");

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.BrandId = dto.BrandId;
            product.CategoryId = dto.CategoryId;

            if (product.Inventory != null)
            {
                product.Inventory.StockQuantity = dto.Stock;
                product.Inventory.LastUpdated = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();

            var updatedProduct = await _db.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Inventory)
                .FirstAsync(p => p.Id == id);

            return MapToDto(updatedProduct);
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _db.Products
                .Include(p => p.Inventory)
                .FirstOrDefaultAsync(p => p.Id == id)
                ?? throw new InvalidOperationException("Product not found.");

            if (product.Inventory != null)
                _db.Inventories.Remove(product.Inventory);

            _db.Products.Remove(product);

            await _db.SaveChangesAsync();
        }

        private static ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Inventory?.StockQuantity ?? 0,
                BrandId = product.BrandId,
                CategoryId = product.CategoryId,
                BrandName = product.Brand?.Name ?? string.Empty,
                CategoryName = product.Category?.Name ?? string.Empty
            };
        }
    }
}