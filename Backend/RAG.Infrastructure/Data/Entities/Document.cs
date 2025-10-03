namespace RAG.Infrastructure.Data.Entities
{
    public class Document
    {
        public int Id {  get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreateAt {  get; set; } = DateTime.UtcNow;
    }
}
