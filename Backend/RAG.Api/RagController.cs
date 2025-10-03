using Microsoft.AspNetCore.Mvc;
using RAG.Core.Interfaces;
using RAG.Core.Models;

namespace RAG.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RagController : ControllerBase
{
    private readonly IRagService _rag;
    public RagController(IRagService rag) => _rag = rag;

    [HttpPost("ask")]
    public async Task<ActionResult<ChatAnswer>> Ask([FromBody] QuestionDto dto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.Question))
            return BadRequest("Kérdés nem lehet üres.");

        var answer = await _rag.AskAsync(dto.Question, ct);
        return Ok(answer);
    }
}

public record QuestionDto(string Question);