using BigShotCore.Data.Dtos;
using BigShotCore.Data.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ChatbotController : ControllerBase
{
    private readonly IChatbotService _chatbotService;

    public ChatbotController(IChatbotService chatbotService)
    {
        _chatbotService = chatbotService;
    }

    [HttpPost("recommendations")]
    public async Task<ActionResult<ChatbotResponseDto>> GetRecommendations([FromBody] ChatbotRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.UserMessage))
            return BadRequest("Message cannot be empty.");

        var response = await _chatbotService.GetRecommendationsAsync(request);
        return Ok(response);
    }
}
