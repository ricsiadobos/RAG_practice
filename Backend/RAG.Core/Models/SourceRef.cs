namespace RAG.Core.Models
{
    public sealed class SourceRef
    {
        public required string Title { get; init; }
        public string? Uri { get; init; }
        public required string Snippet { get; init; }
    }
}
