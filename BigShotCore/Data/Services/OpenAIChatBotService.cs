
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
                var keywords = request.UserMessage.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                // Simple keyword-based search
                var recommendedProducts = await _db.Products
                    .Where(p => keywords.Any(k =>
                        p.Name.Contains(k, StringComparison.OrdinalIgnoreCase) ||
                        p.ShortDescription.Contains(k, StringComparison.OrdinalIgnoreCase)))
                    .Take(5)
                    .ToListAsync();

                var productDtos = recommendedProducts.Select(p => p.ToDto());

                var reply = recommendedProducts.Any()
                    ? "Here are some products you might like:"
                    : "Sorry, we couldn't find any products matching your query.";

                return new ChatbotResponseDto(reply, productDtos);
            }
        }
    }
