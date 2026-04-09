using System.Net.Http.Headers;
using LlmWrapper.Api.Services;
using LlmWrapper.Api.Services.Groq;

var builder = WebApplication.CreateBuilder(args);

// Groq API key
var groqApiKey = builder.Configuration["Groq:ApiKey"];
if (string.IsNullOrWhiteSpace(groqApiKey))
    throw new InvalidOperationException("Groq:ApiKey is not configured. Set it via appsettings or user secrets.");

// Typed HttpClient → GroqClient
builder.Services.AddHttpClient<ILlmClient, GroqClient>(client =>
{
    client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", groqApiKey);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddScoped<LlmService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection();
app.MapControllers();

app.Run();
