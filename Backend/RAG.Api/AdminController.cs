using Microsoft.AspNetCore.Mvc;
using RAG.Core.Interfaces;
using RAG.Core.Models;

namespace RAG.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IAiProvider _ai;
    private readonly IEmbeddingStore _store;

    public AdminController(IAiProvider ai, IEmbeddingStore store)
    {
        _ai = ai;
        _store = store;
    }

    // Egyszerű seed: 2 rövid „szabály”
    [HttpPost("seed")]
    public async Task<IActionResult> Seed(CancellationToken ct)
    {
        // 1) gondoskodjunk róla, hogy a store inicializált legyen
        await _store.InitAsync(ct);

        // 2) dummy tartalmak
        var chunks = new List<DocChunk>
        {
            new DocChunk {
                Id = "policy_ho_v1_c1",
                DocId = "dummy_policy:v1",
                Text = "A munkavállaló heti legfeljebb 3 napot dolgozhat távmunkában előzetes vezetői jóváhagyással.",
                Meta = new Dictionary<string,string> { ["title"] = "Belső szabályzat v1 - Home Office", ["docId"] = "dummy_policy:v1" }
            },
            new DocChunk {
                Id = "mt_szabadsag_2025_c1",
                DocId = "mt:2025-01-01",
                Text = "A munkavállalónak évente 20 munkanap alapszabadság jár. A pótszabadság az életkor és egyéb körülmények szerint növekedhet.",
                Meta = new Dictionary<string,string> { ["title"] = "Munkatörvénykönyv – Szabadság (2025-01-01)", ["docId"] = "mt:2025-01-01" }
            }
        };

        // 3) embedding + index
        var items = new List<(DocChunk chunk, float[] vec)>(chunks.Count);
        foreach (var c in chunks)
        {
            var vec = await _ai.EmbedAsync(c.Text, ct);
            items.Add((c, vec));
        }
        await _store.IndexAsync(items, ct);

        return Ok(new { indexed = items.Count });
    }
}
