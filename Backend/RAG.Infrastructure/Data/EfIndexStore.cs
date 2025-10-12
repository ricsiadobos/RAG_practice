using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using Microsoft.EntityFrameworkCore;
using RAG.Core;
using RAG.Infrastructure.Data.Entities;

namespace RAG.Infrastructure.Data
{
    public class EfIndexStore : IIndexStore
    {
        private readonly ApplicationDbContext _db;

        public EfIndexStore(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<int> AddDokumentAsync(AddDocumentRequest request, CancellationToken ct)
        {
            var doc = new Document
            {
                Title = request.Title,
                Content = request.Content,
                CreateAt = DateTime.UtcNow
            };
            
            _db.Documents.Add(doc);
            await _db.SaveChangesAsync(ct);
            return doc.Id;
        }

        public async Task<int> AddChuckAsync(AddChunkRequest request, CancellationToken ct)
        {
            var chunk = new Chunk
            {
                DocumentId = request.DocumentId,
                Ordinal = request.Ordinal,
                Text = request.Text,
                CharStart = request.CharStart,
                CharEnd = request.CharEnd,
                CreatedAt = DateTime.UtcNow
            };

            _db.Chunks.Add(chunk);
            await _db.SaveChangesAsync(ct);
            return chunk.Id;
        }
        public async Task AddEmbeddingAsync(AddEmbeddingRequest request, CancellationToken ct)
        {
            var bytes = Data.VectorSerializer.ToBytes(request.Vector);

            var emb = await _db.Embeddings
                            .FirstOrDefaultAsync(e => e.ChunkId == request.ChunkId, ct);

            if (emb == null)
            {
                emb = new Embedding
                {
                    ChunkId = request.ChunkId,
                    Model = request.Model,
                    Dim = request.Dim,
                    Vector = bytes,
                    CreatedAt = DateTime.UtcNow
                };
            }
            else
            {
                emb.Model = request.Model;
                emb.Dim = request.Dim;
                emb.Vector = bytes;
            }

            await _db.SaveChangesAsync(ct);
        }
    }
}
