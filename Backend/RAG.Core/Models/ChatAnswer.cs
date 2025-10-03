namespace RAG.Core.Models
{
    public sealed class ChatAnswer
    {
        public required string AnswerHtml { get; init; }
        public required List<SourceRef> Sources { get; init; }
    }
}
