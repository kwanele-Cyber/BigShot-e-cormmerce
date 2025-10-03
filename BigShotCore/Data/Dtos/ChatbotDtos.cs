namespace BigShotCore.Data.Dtos
{
    public record ChatbotRequestDto(
        string UserMessage
    );

    public record ChatbotResponseDto(
        string Reply,
        IEnumerable<ProductDto> Recommendations
    );
}
