using System.Security.Cryptography;
using System.Text;
using RAG.Core.Interfaces;

namespace RAG.Infrastructure;

public sealed class FakeAiProvider : IAiProvider
{
    public Task<string> ChatAsync(string system, string prompt, int maxTokens, double temperature, CancellationToken ct)
    {
        // Csak demonstráció: a prompt elejét visszaírjuk „válaszként”
        var preview = prompt.Length > 300 ? prompt[..300] + "..." : prompt;
        var response =
        $@"[FAKE AI VÁLASZ]
        System: {system}
        Prompt-előnézet: 
        {preview}

        (Meet: ez csak helykitöltő válasz, valódi LLM-hívás nélkül.)";

        return Task.FromResult(response);
    }

    public Task<float[]> EmbedAsync(string text, CancellationToken ct)
    {
        // Determinisztikus „pszeudo-embedding” (NE használd élesben)
        // SHA256 hash -> 16 float érték 0..1 között
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(text));
        var vec = new float[16];
        for (int i = 0; i < vec.Length; i++)
        {
            // 2 byte -> ushort -> normálás
            int b0 = bytes[(i * 2) % bytes.Length];
            int b1 = bytes[(i * 2 + 1) % bytes.Length];
            int v = (b0 << 8) | b1;
            vec[i] = v / 65535f;
        }
        return Task.FromResult(vec);
    }
}