using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using Microsoft.Extensions;


namespace BigShotCore.Data.Models
{

    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Url(ErrorMessage = "ImageUrl must be a valid URL.")]
        [MaxLength(500, ErrorMessage = "Image URL cannot exceed 500 characters.")]
        public string ImageUrl { get; set; } = "";

        [Required(ErrorMessage = "Product name is required.")]
        [MaxLength(250, ErrorMessage = "Product name cannot exceed 250 characters.")]
        public string Name { get; set; } = "";

        // Short plain text description
        [Required(ErrorMessage = "Short description is required.")]
        [MaxLength(500, ErrorMessage = "Short description cannot exceed 500 characters.")]
        public string ShortDescription { get; set; }

        // Store in DB as Markdown
        [Required(ErrorMessage = "Markdown description is required.")]
        public string LongDescriptionMarkdown { get; set; }

        // Store rendered HTML (generated from Markdown)
        public string LongDescriptionHtml { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public double Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock must be zero or a positive number.")]
        public int InStock { get; set; }

        [Range(0.0, 5.0, ErrorMessage = "Rating must be between 0 and 5.")]
        public double Rating { get; set; }
    }

}
