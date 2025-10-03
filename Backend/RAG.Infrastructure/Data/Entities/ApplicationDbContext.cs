using Microsoft.EntityFrameworkCore;

namespace RAG.Infrastructure.Data.Entities
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Document> Documents { get; set; } = null!;
        public DbSet<Chunk> Chunks { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Document>(e =>
            {
                e.HasKey(d => d.Id);
                e.Property(d => d.Title).HasMaxLength(200);
            });

            modelBuilder.Entity<Chunk>(e =>
            {
                e.HasKey(c => c.Id);
                e.HasOne(c => c.Document)
                 .WithMany() 
                 .HasForeignKey(c => c.DocumentId)
                 .OnDelete(DeleteBehavior.Cascade);
                e.HasIndex(c => new { c.DocumentId, c.Ordinal });
            });
        }
    }
}
