using BigShotCore.Data.Dtos;
using Microsoft.AspNetCore.Mvc;
using BigShotCore.Data.Services;
using BigShotCore.Data.Models;
using BigShotCore.Extensions;

namespace BigShotApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // -------------------- CRUD --------------------

        // Anyone can list products
        [HttpGet("list")]
        [AuthorizeRole("Admin", "Customer")]
        public async Task<IActionResult> ListProducts(int pageSize = 10, int pageIndex = 0)
        {
            var products = await _productService.ListProductsAsync(pageSize, pageIndex);
            return Ok(products);
        }

        [HttpGet("{id}")]
        [AuthorizeRole("Admin", "Customer")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        // Only Admins can add products
        [HttpPost]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> AddProduct(CreateProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                ShortDescription = dto.ShortDescription,
                LongDescriptionMarkdown = dto.LongDescriptionMarkdown,
                Price = dto.Price,
                InStock = dto.InStock,
                ImageUrl = dto.ImageUrl,
                Rating = dto.Rating
            };

            var added = await _productService.AddProductAsync(product);
            return Ok(added.ToDto());
        }

        // Only Admins can update products
        [HttpPut("{id}")]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> UpdateProduct(int id, UpdateProductDto dto)
        {
            var updated = await _productService.UpdateProductAsync(id, dto);
            return updated ? Ok() : NotFound();
        }

        // Only Admins can delete products
        [HttpDelete("{id}")]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var deleted = await _productService.DeleteProductAsync(id);
            return deleted ? Ok() : NotFound();
        }

        // -------------------- Utilities --------------------

        [HttpGet("search")]
        [AuthorizeRole("Admin", "Customer")]
        public async Task<IActionResult> SearchProducts(string keyword, int pageSize = 10, int pageIndex = 0)
        {
            var products = await _productService.SearchProductsAsync(keyword, pageSize, pageIndex);
            return Ok(products);
        }

        [HttpGet("filter")]
        [AuthorizeRole("Admin", "Customer")]
        public async Task<IActionResult> FilterProducts(double minPrice, double maxPrice, int pageSize = 10, int pageIndex = 0)
        {
            var products = await _productService.FilterByPriceAsync(minPrice, maxPrice, pageSize, pageIndex);
            return Ok(products);
        }

        [HttpGet("instock")]
        [AuthorizeRole("Admin", "Customer")]
        public async Task<IActionResult> GetInStockProducts(int pageSize = 10, int pageIndex = 0)
        {
            var products = await _productService.GetInStockAsync(pageSize, pageIndex);
            return Ok(products);
        }

        [HttpGet("toprated")]
        [AuthorizeRole("Admin", "Customer")]
        public async Task<IActionResult> GetTopRatedProducts(int count = 5)
        {
            var products = await _productService.GetTopRatedAsync(count);
            return Ok(products);
        }
    }
}
