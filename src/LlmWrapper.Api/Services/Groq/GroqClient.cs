using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace LlmWrapper.Api.Services.Groq;

public class GroqClient : ILlmClient
{
    private const string ChatCompletionsEndpoint = "https://api.groq.com/openai/v1/chat/completions";

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    private readonly HttpClient _httpClient;
    private readonly ILogger<GroqClient> _logger;

    public GroqClient(HttpClient httpClient, ILogger<GroqClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<LlmResult> CompleteAsync(
        string userMessage,
        string? systemPrompt,
        string? model,
        float temperature,
        CancellationToken ct)
    {
        var messages = new List<GroqMessage>();

        if (systemPrompt is not null)
            messages.Add(new GroqMessage("system", systemPrompt));

        messages.Add(new GroqMessage("user", userMessage));

        var request = new GroqRequest(
            Model: model ?? "llama-3.3-70b-versatile",
            Messages: messages,
            Temperature: temperature
        );

        _logger.LogInformation(
            "Sending request to Groq — model: {Model}, estimated input messages: {MessageCount}",
            request.Model, messages.Count);

        var response = await _httpClient.PostAsJsonAsync(
            ChatCompletionsEndpoint, request, SerializerOptions, ct);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(ct);
            throw new HttpRequestException(
                $"Groq API returned {(int)response.StatusCode}: {errorBody}",
                inner: null,
                statusCode: response.StatusCode);
        }

        var groqResponse = await response.Content.ReadFromJsonAsync<GroqResponse>(
            SerializerOptions, ct)
            ?? throw new InvalidOperationException("Groq API returned an empty response.");

        var choice = groqResponse.Choices[0];

        return new LlmResult(
            Content: choice.Message.Content,
            Model: groqResponse.Model,
            PromptTokens: groqResponse.Usage.PromptTokens,
            CompletionTokens: groqResponse.Usage.CompletionTokens
        );
    }
}
