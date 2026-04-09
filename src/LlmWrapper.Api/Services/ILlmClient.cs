namespace LlmWrapper.Api.Services;

public interface ILlmClient
{
    Task<LlmResult> CompleteAsync(
        string userMessage,
        string? systemPrompt,
        string? model,
        float temperature,
        CancellationToken ct);
}

public record LlmResult(
    string Content,
    string Model,
    int PromptTokens,
    int CompletionTokens
);
