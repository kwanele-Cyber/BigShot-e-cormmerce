
using BigShotCore.Data.Dtos;
using BigShotCore.Data.Models;
using BigShotCore.Extensions;
using BigShotCore.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace BigShotCore.Data.Services
{
    public class ChatbotService : IChatbotService
    {
        private readonly AppDbContext _db;

        public ChatbotService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<ChatbotResponseDto> GetRecommendationsAsync(ChatbotRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.UserMessage))
            {
                return new ChatbotResponseDto(
                    SystemReply: "Please provide a search term.",
                    AiReply: "",
                    Enumerable.Empty<ProductDto>());
            }

            var keywords = request.UserMessage.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            IQueryable<Product> query = _db.Products;

            // Apply LIKE filtering for each keyword
            foreach (var keyword in keywords)
            {
                string pattern = $"%{keyword}%";
                query = query.Where(p =>
                    EF.Functions.Like(p.Name, pattern) ||
                    EF.Functions.Like(p.ShortDescription, pattern));
            }

            var recommendedProducts = await query
                .OrderByDescending(p => p.Rating) // prioritize higher rated products
                .Take(5)
                .ToListAsync();

            var productDtos = recommendedProducts.Select(p => p.ToDto());

            var reply = recommendedProducts.Any()
                ? "Here are some products you might like:"
                : "Sorry, we couldn't find any products matching your query.";

            return new ChatbotResponseDto(SystemReply: reply, AiReply: "", Recommendations: productDtos);
        }


    }

}
