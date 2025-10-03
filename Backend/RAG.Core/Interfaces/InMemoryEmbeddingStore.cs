using System.Collections.Concurrent;
using RAG.Core.Interfaces;
using RAG.Core.Models;

namespace RAG.Infrastructure;

public sealed class InMemoryEmbeddingStore : IEmbeddingStore
{
    // chunk_id -> (chunk, vec)
    private readonly ConcurrentDictionary<string, (DocChunk chunk, float[] vec)> _index = new();

    public Task InitAsync(CancellationToken ct)
    {
        // Nincs teendő memória-alapú tárolónál
        return Task.CompletedTask;
    }

    public Task IndexAsync(IEnumerable<(DocChunk chunk, float[] vec)> items, CancellationToken ct)
    {
        foreach (var (chunk, vec) in items)
        {
            // védelem: null/üres szövegű chunkot nem indexelünk
            if (string.IsNullOrWhiteSpace(chunk.Text)) continue;

            // opcionális normalizálás (cosine gyorsít, stabilitást ad)
            var norm = Normalize(vec);
            _index[chunk.Id] = (chunk, norm);
        }
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<ScoredChunk>> SearchHybridAsync(string query, float[] qvec, int topK, CancellationToken ct)
    {
        // egyszerű „hibrid” pontszám:
        // score = 0.5 * cosine(qvec, vec) + 0.5 * textMatchScore(query, chunk.Text)
        var qn = Normalize(qvec);
        var results = new List<ScoredChunk>(_index.Count);

        foreach (var kv in _index)
        {
            var (chunk, vec) = kv.Value;

            // cosine rész
            var cos = Cosine(qn, vec);

            // nagyon egyszerű szöveg „BM25” pótlék: részsztring-keresés
            var textScore = TextMatchScore(query, chunk.Text);

            var score = 0.5 * cos + 0.5 * textScore;

            results.Add(new ScoredChunk
            {
                Id = chunk.Id,
                DocId = chunk.DocId,
                Text = chunk.Text,
                Meta = chunk.Meta,
                Score = score
            });
        }

        var top = results
            .OrderByDescending(r => r.Score)
            .Take(Math.Max(1, topK))
            .ToList()
            .AsReadOnly();

        return Task.FromResult<IReadOnlyList<ScoredChunk>>(top);
    }

    // --- segédfüggvények ---

    private static float[] Normalize(float[] v)
    {
        double sum = 0;
        for (int i = 0; i < v.Length; i++) sum += v[i] * v[i];
        var norm = (float)Math.Sqrt(sum);
        if (norm == 0) return v.ToArray();
        var outv = new float[v.Length];
        for (int i = 0; i < v.Length; i++) outv[i] = v[i] / norm;
        return outv;
    }

    private static double Cosine(float[] a, float[] b)
    {
        var n = Math.Min(a.Length, b.Length);
        double dot = 0;
        for (int i = 0; i < n; i++) dot += a[i] * b[i];
        // a és b elvileg normáltak
        return dot;
    }

    private static double TextMatchScore(string query, string text)
    {
        if (string.IsNullOrWhiteSpace(query) || string.IsNullOrWhiteSpace(text)) return 0;
        var q = query.Trim();
        // nagyon egyszerű: tartalmazza-e a teljes query-t (kis-nagybetű független)
        var contains = text.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0;
        if (contains) return 1.0;

        // fallback: token-arány
        var qTokens = q.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (qTokens.Length == 0) return 0;
        int hits = 0;
        foreach (var t in qTokens)
            if (text.IndexOf(t, StringComparison.OrdinalIgnoreCase) >= 0) hits++;

        return (double)hits / qTokens.Length; // 0..1 között
    }
}
