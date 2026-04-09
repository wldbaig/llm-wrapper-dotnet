using System.Text.Json.Serialization;

namespace LlmWrapper.Api.Services.Groq;

internal record GroqRequest(
    string Model,
    List<GroqMessage> Messages,
    float Temperature,
    [property: JsonPropertyName("max_tokens")] int MaxTokens = 1024
);

internal record GroqMessage(
    string Role,
    string Content
);

internal record GroqResponse(
    string Id,
    List<GroqChoice> Choices,
    GroqUsage Usage,
    string Model
);

internal record GroqChoice(
    GroqMessage Message,
    string FinishReason
);

internal record GroqUsage(
    int PromptTokens,
    int CompletionTokens,
    int TotalTokens
);
