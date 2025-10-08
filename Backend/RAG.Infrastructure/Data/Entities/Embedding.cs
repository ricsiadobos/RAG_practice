namespace RAG.Infrastructure.Data.Entities
{
    public class Embedding
    {
        public int Id { get; set; }
        public int ChunkId { get; set; }
        public Chunk Chuck { get; set; }
        public string Model { get; set; } = "";
        public int Dim { get; set; }
        public byte[] Vector { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
