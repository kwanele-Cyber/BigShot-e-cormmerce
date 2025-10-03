using BigShotCore.Data.Dtos;
using BigShotCore.Data.Models;
using Humanizer;
using Markdig;
using System.Xml.Linq;

namespace BigShotCore.Extensions
{
    // Extension method class
    public static class ProductExtensions
    {
        public static Product ToEntity(this CreateProductDto dto)
        {
            return new Product
            {
                Name = dto.Name,
                ShortDescription = dto.ShortDescription,
                LongDescriptionMarkdown = dto.LongDescriptionMarkdown,
                LongDescriptionHtml = Markdown.ToHtml(dto.LongDescriptionMarkdown ?? ""),
                Price = dto.Price,
                ImageUrl = dto.ImageUrl,
                InStock = dto.InStock,
                Rating = dto.Rating
            };
        }

        public static ProductDto ToDto(this Product product)
        {
            return new ProductDto(
                product.Id,
                product.Name,
                product.ShortDescription,
                product.LongDescriptionMarkdown,
                product.LongDescriptionHtml,
                product.Price,
                product.ImageUrl,
                product.InStock,
                product.Rating

            );
        }


        public static Product UpdateFromDto(this Product product, UpdateProductDto dto)
        {
            // Only overwrite if not null
            product.Name = dto.Name ?? product.Name;
            product.ShortDescription = dto.ShortDescription ?? product.ShortDescription;
            product.LongDescriptionMarkdown = dto.LongDescriptionMarkdown ?? product.LongDescriptionMarkdown;
            product.LongDescriptionHtml = Markdown.ToHtml(product.LongDescriptionMarkdown ?? "");
            product.Price = dto.Price ?? product.Price;
            product.ImageUrl = dto.ImageUrl ?? product.ImageUrl;
            product.InStock = dto.InStock ?? product.InStock;
            product.Rating = dto.Rating ?? product.Rating;
            return product;
        }

        public static IEnumerable<ProductDto> ToDtos(this IEnumerable<Product> products)
        {
            return products.Select(p => p.ToDto());
        }

    }
}
