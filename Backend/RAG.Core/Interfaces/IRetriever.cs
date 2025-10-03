using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RAG.Core.Models;

namespace RAG.Core.Interfaces;

public interface IRetriever
{
    Task<IReadOnlyList<ScoredChunk>> RetrieveAsync(string question, int topK, CancellationToken ct);
}
