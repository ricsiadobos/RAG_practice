namespace RAG.Infrastructure.Data.Entities
{
    public class Chunk
    {
        public int Id { get; set; }            
        public int DocumentId { get; set; }    
        public Document Document { get; set; } = null!;
        public int Ordinal { get; set; }       
        public string Text { get; set; } = "";
        public int CharStart { get; set; }
        public int CharEnd { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
