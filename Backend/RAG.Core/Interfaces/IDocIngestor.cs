namespace RAG.Core.Interfaces
{
    public interface IDocIngestor
    {
        Task IngestAsync(string[] filePaths, string sourceTag, string version, DateOnly validFrom, CancellationToken ct);
    }
}
