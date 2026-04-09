using System.Net;
using System.Text;
using System.Text.Json;
using LlmWrapper.Api.Services.Groq;
using Microsoft.Extensions.Logging.Abstractions;

namespace LlmWrapper.Tests;

public class GroqClientTests
{
    private static readonly JsonSerializerOptions SnakeCase = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    private static HttpClient BuildClient(HttpStatusCode status, string body)
    {
        var handler = new FakeHttpMessageHandler(status, body);
        return new HttpClient(handler);
    }

    private static string ValidGroqJson(string content = "Hello!", string model = "llama-3.3-70b-versatile") =>
        JsonSerializer.Serialize(new
        {
            id = "chatcmpl-123",
            model,
            choices = new[]
            {
                new
                {
                    message = new { role = "assistant", content },
                    finish_reason = "stop"
                }
            },
            usage = new
            {
                prompt_tokens = 10,
                completion_tokens = 20,
                total_tokens = 30
            }
        });

    [Fact]
    public async Task CompleteAsync_ReturnsLlmResult_WhenApiReturnsValidResponse()
    {
        var expectedContent = "A large language model is a neural network trained on vast text data.";
        var expectedModel = "llama-3.3-70b-versatile";

        var client = BuildClient(HttpStatusCode.OK, ValidGroqJson(expectedContent, expectedModel));
        var groqClient = new GroqClient(client, NullLogger<GroqClient>.Instance);

        var result = await groqClient.CompleteAsync(
            userMessage: "What is an LLM?",
            systemPrompt: "You are helpful.",
            model: expectedModel,
            temperature: 0.7f,
            ct: CancellationToken.None);

        Assert.Equal(expectedContent, result.Content);
        Assert.Equal(expectedModel, result.Model);
        Assert.Equal(10, result.PromptTokens);
        Assert.Equal(20, result.CompletionTokens);
    }

    [Fact]
    public async Task CompleteAsync_ThrowsHttpRequestException_WhenApiReturns429()
    {
        var client = BuildClient(HttpStatusCode.TooManyRequests, "{\"error\":\"rate limit exceeded\"}");
        var groqClient = new GroqClient(client, NullLogger<GroqClient>.Instance);

        var ex = await Assert.ThrowsAsync<HttpRequestException>(() =>
            groqClient.CompleteAsync(
                userMessage: "Hello",
                systemPrompt: null,
                model: null,
                temperature: 0.7f,
                ct: CancellationToken.None));

        Assert.Equal(HttpStatusCode.TooManyRequests, ex.StatusCode);
        Assert.Contains("429", ex.Message);
    }

    private sealed class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _statusCode;
        private readonly string _body;

        public FakeHttpMessageHandler(HttpStatusCode statusCode, string body)
        {
            _statusCode = statusCode;
            _body = body;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent(_body, Encoding.UTF8, "application/json")
            });
    }
}
