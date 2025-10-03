using RAG.Core.Models;

namespace RAG.Core.Interfaces
{
    public interface IRagService
    {
        Task<ChatAnswer> AskAsync(string question, CancellationToken ct);
    }
}
