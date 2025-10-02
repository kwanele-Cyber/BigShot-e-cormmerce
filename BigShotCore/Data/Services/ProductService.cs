using BigShotCore.Extensions;
using BigShotCore.Data.Dtos;
using BigShotCore.Data.Models;
using BigShotCore.Infrastructure.Database;

using Markdig;

using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BigShotCore.Data.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _db;

        public ProductService(AppDbContext db)
        {
            _db = db;
        }

        // -------------------- CRUD --------------------

        public async Task<Product> AddProductAsync(Product product)
        {
            product.LongDescriptionHtml = Markdown.ToHtml(product.LongDescriptionMarkdown ?? "");
            await _db.Products.AddAsync(product);
            await _db.SaveChangesAsync();
            return product;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return false;

            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == id);
            return product?.ToDto();
        }

        public async Task<bool> UpdateProductAsync(int id, UpdateProductDto dto)
        {
            var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return false;

            product.UpdateFromDto(dto);
            product.LongDescriptionHtml = Markdown.ToHtml(product.LongDescriptionMarkdown ?? "");
            await _db.SaveChangesAsync();
            return true;
        }

        // -------------------- Listing / Utilities --------------------

        public async Task<IEnumerable<ProductDto>> ListProductsAsync(int pageSize, int pageIndex)
        {
            var products = await _db.Products
                                    .OrderBy(p => p.Id)
                                    .Skip(pageIndex * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync();

            return products.ToDtos();
        }

        public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string keyword, int pageSize, int pageIndex)
        {
            var products = await _db.Products
                                    .Where(p => p.Name.Contains(keyword) || p.ShortDescription.Contains(keyword))
                                    .OrderBy(p => p.Id)
                                    .Skip(pageIndex * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync();

            return products.ToDtos();
        }

        public async Task<IEnumerable<ProductDto>> FilterByPriceAsync(double minPrice, double maxPrice, int pageSize, int pageIndex)
        {
            var products = await _db.Products
                                    .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
                                    .OrderBy(p => p.Price)
                                    .Skip(pageIndex * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync();

            return products.ToDtos();
        }

        public async Task<IEnumerable<ProductDto>> GetInStockAsync(int pageSize, int pageIndex)
        {
            var products = await _db.Products
                                    .Where(p => p.InStock > 0)
                                    .OrderBy(p => p.Id)
                                    .Skip(pageIndex * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync();

            return products.ToDtos();
        }

        public async Task<IEnumerable<ProductDto>> GetTopRatedAsync(int count)
        {
            var products = await _db.Products
                                    .OrderByDescending(p => p.Rating)
                                    .Take(count)
                                    .ToListAsync();

            return products.ToDtos();
        }
    }
}
