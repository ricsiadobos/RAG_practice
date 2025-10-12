using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAG.Core
{
    public sealed record AddDocumentRequest(string Title, string Content);

    public sealed record AddChunkRequest(
        int DocumentId,
        int Ordinal,
        string Text,
        int CharStart,
        int CharEnd
    );

    public sealed record AddEmbeddingRequest(
        int ChunkId,
        string Model,
        int Dim,
        float[] Vector
    );
}
