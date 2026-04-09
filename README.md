# LlmWrapper

A clean .NET 9 REST API that wraps LLM providers behind a provider-agnostic `ILlmClient` abstraction. Currently ships with a [Groq](https://groq.com/) implementation using `llama-3.3-70b-versatile`.

## Project Structure

```
src/
  LlmWrapper.Api/
    Controllers/       # ChatController — POST /api/chat
    Models/            # ChatRequest, ChatResponse
    Services/
      ILlmClient.cs    # Provider abstraction + LlmResult record
      LlmService.cs    # Orchestration layer (timing, logging)
      Groq/
        GroqClient.cs  # ILlmClient implementation for Groq
        GroqModels.cs  # Internal Groq request/response models
tests/
  LlmWrapper.Tests/   # xUnit test project
```

## API

### `POST /api/chat`

**Request body:**
```json
{
  "message": "What is the capital of France?",
  "systemPrompt": "You are a helpful assistant.",
  "model": "llama-3.3-70b-versatile",
  "temperature": 0.7
}
```
`systemPrompt`, `model`, and `temperature` are optional. Default model is `llama-3.3-70b-versatile`.

**Response:**
```json
{
  "content": "The capital of France is Paris.",
  "model": "llama-3.3-70b-versatile",
  "promptTokens": 24,
  "completionTokens": 10,
  "latencyMs": 312
}
```

Swagger UI is available at `/swagger` when running locally.

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- A [Groq API key](https://console.groq.com/)

### Configuration

Set your Groq API key via .NET user secrets (recommended for local dev):

```bash
cd src/LlmWrapper.Api
dotnet user-secrets set "Groq:ApiKey" "your-api-key-here"
```

Or add it to `appsettings.Development.json`:
```json
{
  "Groq": {
    "ApiKey": "your-api-key-here"
  }
}
```

### Run

```bash
cd src/LlmWrapper.Api
dotnet run
```

### Test

```bash
dotnet test
```

## Adding a New LLM Provider

1. Implement `ILlmClient` in a new class under `Services/<ProviderName>/`
2. Register it in `Program.cs` via `AddHttpClient<ILlmClient, YourClient>(...)`

No other changes needed — `LlmService` and `ChatController` are provider-agnostic.
