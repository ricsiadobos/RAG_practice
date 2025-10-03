namespace RAG.Core.Interfaces
{
    public interface IAiProvider
    {
        Task<string> ChatAsync(string system, string prompt, int maxTokens, double temperature, CancellationToken ct);
        Task<float[]> EmbedAsync(string text, CancellationToken ct);
    }
}
