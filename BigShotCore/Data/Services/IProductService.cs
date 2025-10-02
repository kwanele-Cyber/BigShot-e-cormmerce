using BigShotCore.Data.Dtos;
using BigShotCore.Data.Models;

using System.Collections.Generic;
using System.Threading.Tasks;



namespace BigShotCore.Data.Services
{
    public interface IProductService
    {
        // -------------------- CRUD --------------------
        Task<Product> AddProductAsync(Product product); // Create
        Task<bool> DeleteProductAsync(int id); // Delete
        Task<ProductDto?> GetProductByIdAsync(int id); // Read single item as DTO
        Task<bool> UpdateProductAsync(int id, UpdateProductDto dto); // Update

        // -------------------- Listing / Utilities --------------------
        Task<IEnumerable<ProductDto>> ListProductsAsync(int pageSize, int pageIndex);
        Task<IEnumerable<ProductDto>> SearchProductsAsync(string keyword, int pageSize, int pageIndex);
        Task<IEnumerable<ProductDto>> FilterByPriceAsync(double minPrice, double maxPrice, int pageSize, int pageIndex);
        Task<IEnumerable<ProductDto>> GetInStockAsync(int pageSize, int pageIndex);
        Task<IEnumerable<ProductDto>> GetTopRatedAsync(int count);
    }
}
