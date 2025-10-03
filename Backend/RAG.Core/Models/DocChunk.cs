using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAG.Core.Models
{
    public class DocChunk
    {
        public required string Id { get; init; }
        public required string DocId { get; init; }
        public required string Text { get; init; }
        public IDictionary<string, string>? Meta { get; init; }
    }
}
