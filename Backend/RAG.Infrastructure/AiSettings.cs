using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAG.Infrastructure
{
    public sealed class AiSettings
    {
        public required string ApiKey { get; init; }
        public string ChatModel { get; init; } = "gpt-4o-mini";
        public string EmbeddingModel { get; init; } = "text-embedding-3-large";
    }
}
