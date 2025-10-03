using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RAG.Core.Models;

namespace RAG.Core.Interfaces
{
    public interface IEmbeddingStore
    {
        Task InitAsync(CancellationToken ct);
        Task IndexAsync(IEnumerable<(DocChunk chunk, float[] vec)> items, CancellationToken ct);
        Task<IReadOnlyList<ScoredChunk>> SearchHybridAsync(string query, float[] qvec, int topK, CancellationToken ct);
    }
}
