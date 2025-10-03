using BigShotCore.Data.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BigShotCore.Data.Services
{
    public interface IChatbotService
    {
        Task<ChatbotResponseDto> GetRecommendationsAsync(ChatbotRequestDto request);
    }
}
