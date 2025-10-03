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
            // Document tábla – opcionális alap beállítások
            modelBuilder.Entity<Document>(e =>
            {
                e.HasKey(d => d.Id);
                e.Property(d => d.Title).HasMaxLength(200);
                // e.Property(d => d.Content) -> nvarchar(max) default
            });

            // Chunk tábla – CSAK skálár mezőkre tegyünk indexet!
            modelBuilder.Entity<Chunk>(e =>
            {
                e.HasKey(c => c.Id);

                // KAPCSOLAT: FK a DocumentId-re
                e.HasOne(c => c.Document)
                 .WithMany() // később tehetünk ICollection<Chunk>-ot a Document-be
                 .HasForeignKey(c => c.DocumentId)
                 .OnDelete(DeleteBehavior.Cascade);

                // INDEX: csak skálár mezők! (NE: c => new { c.Document })
                e.HasIndex(c => new { c.DocumentId, c.Ordinal });
            });
        }
    }
}
