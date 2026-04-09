using System.Diagnostics;
using LlmWrapper.Api.Models;
using Microsoft.Extensions.Logging;

namespace LlmWrapper.Api.Services;

public class LlmService
{
    private readonly ILlmClient _llmClient;
    private readonly ILogger<LlmService> _logger;

    public LlmService(ILlmClient llmClient, ILogger<LlmService> logger)
    {
        _llmClient = llmClient;
        _logger = logger;
    }

    public async Task<ChatResponse> ChatAsync(ChatRequest request, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();

        var result = await _llmClient.CompleteAsync(
            userMessage: request.Message,
            systemPrompt: request.SystemPrompt,
            model: request.Model,
            temperature: request.Temperature,
            ct: ct);

        sw.Stop();

        _logger.LogInformation(
            "Groq call complete — model: {Model}, prompt tokens: {PromptTokens}, " +
            "completion tokens: {CompletionTokens}, latency: {LatencyMs}ms",
            result.Model, result.PromptTokens, result.CompletionTokens, sw.ElapsedMilliseconds);

        return new ChatResponse(
            Content: result.Content,
            Model: result.Model,
            PromptTokens: result.PromptTokens,
            CompletionTokens: result.CompletionTokens,
            LatencyMs: sw.ElapsedMilliseconds
        );
    }
}
