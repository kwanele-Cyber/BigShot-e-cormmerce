namespace BigShotCore.Data.Dtos
{
    public record ChatbotRequestDto(
        string UserMessage
    );

    public record ChatbotResponseDto(
        string SystemReply,
        string? AiReply,               // AI-generated message
        IEnumerable<ProductDto> Recommendations
    );

}
