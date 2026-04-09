using LlmWrapper.Api.Models;
using LlmWrapper.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace LlmWrapper.Api.Controllers;

[ApiController]
[Route("api/chat")]
public class ChatController : ControllerBase
{
    private readonly LlmService _llmService;

    public ChatController(LlmService llmService)
    {
        _llmService = llmService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ChatResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Chat([FromBody] ChatRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
            return BadRequest("Message must not be empty.");

        try
        {
            var response = await _llmService.ChatAsync(request, ct);
            return Ok(response);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
        }
    }
}
