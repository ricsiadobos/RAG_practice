using RAG.Core.Models;

namespace RAG.Core.Interfaces
{
    public interface IChunker
    {
        IEnumerable<DocChunk> CreateChunks(string documentText, IDictionary<string, string> meta);
    }
}
