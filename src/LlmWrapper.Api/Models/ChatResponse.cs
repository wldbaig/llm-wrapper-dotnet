namespace LlmWrapper.Api.Models;

public record ChatResponse(
    string Content,
    string Model,
    int PromptTokens,
    int CompletionTokens,
    long LatencyMs
);
