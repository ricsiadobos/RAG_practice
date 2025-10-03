using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RAG.Core.Interfaces;
using RAG.Core.Models;

namespace RAG.Infrastructure.Rag
{
    public sealed class RagService : IRagService
    {
        private readonly IRetriever _retriever;
        private readonly IAiProvider _ai;

        public RagService(IRetriever retriever, IAiProvider ai)
        {
            _retriever = retriever;
            _ai = ai;
        }

        public async Task<ChatAnswer> AskAsync(string question, CancellationToken ct)
        {
            // 1) Top K releváns részlet lekérése
            var chunks = await _retriever.RetrieveAsync(question, topK: 5, ct);

            if (chunks.Count == 0)
            {
                return new ChatAnswer
                {
                    AnswerHtml = "Nem áll rendelkezésemre elég információ.",
                    Sources = new()
                };
            }

            // 2) Prompt összeállítás (kontekstukeretben forrásrészletek)
            var prompt = BuildPrompt(question, chunks);

            // 3) LLM hívás
            var system = "Te egy precíz HR jogi asszisztens vagy. Mindig hivatkozz a forrásokra és ha kevés az információ, mondd ki.";
            var llmAnswer = await _ai.ChatAsync(system, prompt, maxTokens: 700, temperature: 0.2, ct);

            // 4) Forráslista összeállítása megjelenítéshez
            var sources = chunks.Select(c => new SourceRef
            {
                Title = c.Meta != null && c.Meta.TryGetValue("title", out var t) ? t : (c.Meta?["docId"] ?? c.DocId),
                Uri = c.Meta != null && c.Meta.TryGetValue("uri", out var u) ? u : null,
                Snippet = c.Text.Length > 240 ? c.Text[..240] + "…" : c.Text
            }).ToList();

            // 5) Válasz vissza
            return new ChatAnswer
            {
                // (V1) sima szöveg – később formázunk HTML-re
                AnswerHtml = llmAnswer,
                Sources = sources
            };
        }

        private static string BuildPrompt(string question, IReadOnlyList<ScoredChunk> chunks)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Feladat: válaszolj tömören, egyértelműen. Csak az alábbi forrásrészletekre támaszkodhatsz.");
            sb.AppendLine("Ha a források alapján nem egyértelmű a válasz, mondd: \"Nem áll rendelkezésemre elég információ.\"");
            sb.AppendLine();
            sb.AppendLine("Kérdés:");
            sb.AppendLine(question);
            sb.AppendLine();
            sb.AppendLine("Források (idézetek):");
            int i = 1;
            foreach (var c in chunks)
            {
                var title = c.Meta != null && c.Meta.TryGetValue("title", out var t) ? t : (c.Meta?["docId"] ?? c.DocId);
                sb.AppendLine($"[{i}] {title}");
                // Rövidítés, hogy a prompt ne legyen túl hosszú
                var text = c.Text;
                if (text.Length > 1000) text = text[..1000] + "…";
                sb.AppendLine(text);
                sb.AppendLine();
                i++;
            }
            sb.AppendLine("Kérlek, a válaszban szögletes zárójelben hivatkozz a forrás sorszámára (pl. [1], [2]).");
            return sb.ToString();
        }
    }
}
