```mermaid

sequenceDiagram
    participant UI as Angular Chat UI
    participant API as ASP.NET Core Web API (/api/rag/ask)
    participant RAG as IRagService (RagService)
    participant RET as IRetriever (Retriever)
    participant STORE as IEmbeddingStore (Embedding t�rol�)
    participant AI as IAiProvider (LLM/Embeddings)

    UI->>API: POST /api/rag/ask { question }
    API->>RAG: AskAsync(question)
    RAG->>RET: RetrieveAsync(question, topK)
    RET->>AI: EmbedAsync(question)  <!-- k�rd�s vektoriz�l�sa -->
    RET->>STORE: SearchHybridAsync(question, qVec, topK)
    STORE-->>RET: topK ScoredChunk (forr�sr�szletek)
    RET-->>RAG: relev�ns chunks
    RAG->>AI: ChatAsync(system, prompt(chunks+question))
    AI-->>RAG: gener�lt v�lasz (text)
    RAG-->>API: ChatAnswer { answerHtml, sources[] }
    API-->>UI: 200 OK + v�lasz + forr�shivatkoz�sok

```

