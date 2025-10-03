using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAG.Core.Models
{
    public class ScoredChunk : DocChunk
    {
        public double Score { get; set; }
    }
}
