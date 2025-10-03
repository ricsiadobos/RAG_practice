```mermaid

flowchart TD
    A[Szöveg] --> B[Tokenizálás]
    B --> C[Alap embedding vektorok]
    C --> D[Transformer rétegek]
    D --> E[Szöveg embedding ]
    E --> F[Vektortér összehasonlítás]
```

```mermaid
sequenceDiagram
title 1. step - Szöveg fogadása

    participant User as Felhasználó
    participant UI as Swagger/Angular UI
    participant API as RAG.Api (AdminController)

    User->>UI: Beírja a szöveget
    UI->>API: POST /Admin/Seed<br/>szöveg elküldése
    API-->>UI: Válasz (pl. sikeres seed)

```

```mermaid
sequenceDiagram
title 2. step - Chunker feldarabolás
    participant API as RAG.Api
    participant Chunker as DocChunker
    participant Core as RAG.Core.Models

    API->>Chunker: Feldarabolás kérése
    Chunker->>Core: Létrehoz DocChunk objektumokat
    Chunker-->>API: Visszaadja a DocChunk listát
```

```mermaid
sequenceDiagram
title 3. step - Embedding készítése
    participant API as RAG.Api
    participant Provider as OpenAiProvider
    participant OpenAI as OpenAI API

    API->>Provider: EmbedAsync("Chunk szöveg")
    Provider->>OpenAI: POST /embeddings
    OpenAI-->>Provider: Visszaad vektort (float[])
    Provider-->>API: Embedding visszaadása
```

```mermaid
sequenceDiagram
title 4. step - Embedding tárolása
    participant API as RAG.Api
    participant Store as InMemoryEmbeddingStore

    API->>Store: Embedding tárolása<br/>DocChunk + vektor
    Store-->>API: Mentés sikeres
```

