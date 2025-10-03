using RAG.Core.Interfaces;
using RAG.Core.Models;

namespace RAG.Infrastructure.Search
{
    public sealed class SimpleRetriever : IRetriever
    {
        private readonly IEmbeddingStore _store;
        private readonly IAiProvider _ai;

        public SimpleRetriever(IEmbeddingStore store, IAiProvider ai)
        {
            _store = store;
            _ai = ai;
        }

        public async Task<IReadOnlyList<ScoredChunk>> RetrieveAsync(string question, int topK, CancellationToken ct)
        {
            // 1) Kérdés beágyazása (vektor)
            var qvec = await _ai.EmbedAsync(question, ct);

            // 2) Hibrid keresés a tárolóban
            var hits = await _store.SearchHybridAsync(question, qvec, topK, ct);

            return hits;
        }
    }
}
