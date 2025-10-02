using System.ComponentModel.DataAnnotations;

namespace BigShotCore.Data.Dtos
{
    // -------------------- Create DTO --------------------
    public record CreateProductDto(
        [Required(ErrorMessage = "Product name is required.")]
        [MaxLength(250, ErrorMessage = "Product name cannot exceed 250 characters.")]
        string Name,

        [Required(ErrorMessage = "Short description is required.")]
        [MaxLength(500, ErrorMessage = "Short description cannot exceed 500 characters.")]
        string ShortDescription,

        [Required(ErrorMessage = "Markdown description is required.")]
        string LongDescriptionMarkdown,

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        double Price,

        [Range(0, int.MaxValue, ErrorMessage = "Stock must be zero or a positive number.")]
        int InStock,

        [Url(ErrorMessage = "ImageUrl must be a valid URL.")]
        [MaxLength(500, ErrorMessage = "Image URL cannot exceed 500 characters.")]
        string ImageUrl,

        [Range(0.0, 5.0, ErrorMessage = "Rating must be between 0 and 5.")]
        double Rating
    );

    // -------------------- Product DTO --------------------
    public record ProductDto(
        int Id,

        [Required(ErrorMessage = "Product name is required.")]
        [MaxLength(250, ErrorMessage = "Product name cannot exceed 250 characters.")]
        string Name,

        [Required(ErrorMessage = "Short description is required.")]
        [MaxLength(500, ErrorMessage = "Short description cannot exceed 500 characters.")]
        string ShortDescription,

        [Required(ErrorMessage = "Markdown description is required.")]
        string LongDescriptionMarkdown,

        string LongDescriptionHtml,

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        double Price,

        [Url(ErrorMessage = "ImageUrl must be a valid URL.")]
        [MaxLength(500, ErrorMessage = "Image URL cannot exceed 500 characters.")]
        string ImageUrl
    );

    // -------------------- Update DTO --------------------
    public record UpdateProductDto(
        [MaxLength(250, ErrorMessage = "Product name cannot exceed 250 characters.")]
        string? Name,

        [MaxLength(500, ErrorMessage = "Short description cannot exceed 500 characters.")]
        string? ShortDescription,

        string? LongDescriptionMarkdown,

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        double? Price,

        [Range(0, int.MaxValue, ErrorMessage = "Stock must be zero or a positive number.")]
        int? InStock,

        [Url(ErrorMessage = "ImageUrl must be a valid URL.")]
        [MaxLength(500, ErrorMessage = "Image URL cannot exceed 500 characters.")]
        string? ImageUrl,

        [Range(0.0, 5.0, ErrorMessage = "Rating must be between 0 and 5.")]
        double? Rating
    );
}
