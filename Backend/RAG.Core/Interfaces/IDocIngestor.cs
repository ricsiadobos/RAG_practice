using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAG.Core.Interfaces
{
    public interface IDocIngestor
    {
        Task IngestAsync(string[] filePaths, string sourceTag, string version, DateOnly validFrom, CancellationToken ct);
    }
}
