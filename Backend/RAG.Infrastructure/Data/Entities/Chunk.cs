using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAG.Infrastructure.Data.Entities
{
    public class Chunk
    {
        public int Id { get; set; }            
        public int DocumentId { get; set; }    
        public Document Document { get; set; } = null!;

        public int Ordinal { get; set; }       
        public string Text { get; set; } = ""; // a chunk szövege
        public int CharStart { get; set; }     // opcionális: karakter start
        public int CharEnd { get; set; }       // opcionális: karakter end
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
