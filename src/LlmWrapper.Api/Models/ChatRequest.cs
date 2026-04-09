namespace LlmWrapper.Api.Models;

public record ChatRequest(
    string Message,
    string? SystemPrompt = null,
    string Model = "llama-3.3-70b-versatile",
    float Temperature = 0.7f
);
