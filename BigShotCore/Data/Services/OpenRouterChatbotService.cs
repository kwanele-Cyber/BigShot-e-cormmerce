using BigShotCore.Data.Dtos;
using BigShotCore.Data.Models;
using BigShotCore.Extensions;
using BigShotCore.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BigShotCore.Data.Services
{
    public class OpenRouterChatbotService : IChatbotService
    {
        private readonly AppDbContext _db;
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _siteUrl;
        private readonly string _siteTitle;
        private readonly string _model;
        private readonly string _endpoint;

        public OpenRouterChatbotService(AppDbContext db, HttpClient httpClient, IConfiguration config)
        {
            _db = db;
            _httpClient = httpClient;
            _apiKey = config["OpenRouter:ApiKey"] ?? throw new ArgumentNullException("OpenRouter:ApiKey not configured");
            _siteUrl = config["OpenRouter:SiteUrl"] ?? "https://your-site.com";
            _siteTitle = config["OpenRouter:SiteTitle"] ?? "BigShot E-Commerce";
            _model = config["OpenRouter:model"] ?? "x-ai/grok-4-fast:free";
            _endpoint = config["OpenRouter:endpoint"] ?? "https://openrouter.ai/api/v1/chat/completions";
        }

        public async Task<ChatbotResponseDto> GetRecommendationsAsync(ChatbotRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.UserMessage))
            {
                return new ChatbotResponseDto(
                    "Please provide a search term.",
                    "I didn’t get any input — could you clarify what product you’re looking for?",
                    Enumerable.Empty<ProductDto>());
            }

            // ---------------- Search products ----------------
            var keywords = request.UserMessage.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            IQueryable<Product> query = _db.Products;

            foreach (var keyword in keywords)
            {
                string pattern = $"%{keyword}%";
                query = query.Where(p =>
                    EF.Functions.Like(p.Name, pattern) ||
                    EF.Functions.Like(p.ShortDescription, pattern));
            }

            var recommendedProducts = await query
                .OrderByDescending(p => p.Rating)
                .Take(5)
                .ToListAsync();

            var productDtos = recommendedProducts.Select(p => p.ToDto());

            var systemReply = recommendedProducts.Any()
                ? "Here are some products you might like:"
                : "Sorry, we couldn't find any products matching your query.";

            // ---------------- Ask AI for response ----------------
            var aiReply = await GetAiResponse(request.UserMessage, recommendedProducts);

            return new ChatbotResponseDto(systemReply, aiReply, productDtos);
        }

        private async Task<string> GetAiResponse(string userMessage, IEnumerable<Product> products)
        {
            var productList = products.Any()
                ? string.Join(", ", products.Select(p => $"{p.Name} (${p.Price})"))
                : "no products found";

            var prompt = $"The user asked: {userMessage}. " +
                         $"We found these products: {productList}. " +
                         $"Please generate a helpful product recommendation response.";

            var requestBody = new
            {
                model = _model,
                messages = new[]
                {
                    new { role = "user", content = prompt }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", _siteUrl);
            _httpClient.DefaultRequestHeaders.Add("X-Title", _siteTitle);

            var response = await _httpClient.PostAsync(_endpoint, content);
            var responseJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                try
                {
                    var doc = JsonDocument.Parse(responseJson);
                    return doc.RootElement.GetProperty("message").GetString()
                           ?? $"AI API error {response.StatusCode}";
                }
                catch
                {
                    return $"AI API error {response.StatusCode}: {responseJson}";
                }
            }

            using var jsonDoc = JsonDocument.Parse(responseJson);
            return jsonDoc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString()
                ?? "AI did not return a response.";
        }
    }
}
