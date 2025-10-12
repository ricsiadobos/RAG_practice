using RAG.Core;

namespace RAG.Infrastructure.Data
{
    public interface IIndexStore
    {
        Task<int> AddDokumentAsync(AddDocumentRequest request, CancellationToken ct);
        Task<int> AddChuckAsync(AddChunkRequest request, CancellationToken ct);
        Task AddEmbeddingAsync(AddEmbeddingRequest request, CancellationToken ct);
    }
}
