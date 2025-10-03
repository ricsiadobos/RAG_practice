```mermaid
sequenceDiagram
title 1. Kérés fogadása
  participant User as Felhasználó
  participant UI as Kliens
  participant API as RAG.Api RagController

  User->>UI: Kérdés beírása
  UI->>API: POST /api/rag/ask
  API-->>UI: 202 Accepted vagy 200 OK válasz majd
```

```mermaid
sequenceDiagram
title 2. Embadding készítés, keresés
  participant API as RagService
  participant RET as SimpleRetriever
  participant AI as IAiProvider
  participant STORE as InMemoryEmbeddingStore

  API->>RET: RetrieveAsync kérdés topK
  RET->>AI: EmbedAsync kérdés szöveg
  AI-->>RET: Kérdés vektor
  RET->>STORE: SearchHybridAsync kérdés szöveg + vektor
  STORE-->>RET: Találatok scored chunks
  RET-->>API: Releváns chunks lista
```

```mermaid
sequenceDiagram
title 3. Prompt összeállítása
  participant API as RagService
  participant R as Forrás lista

  API->>API: Források rövidítése
  API->>API: Prompt összeállítás kérdés + források
  API-->>R: Forráshivatkozások listája
```

```mermaid
sequenceDiagram
Title 4. Válasz generálása a chat modellel
  participant API as RagService
  participant AI as IAiProvider
  participant LLM as Chat modell

  API->>AI: ChatAsync system szabályok + prompt
  AI->>LLM: Chat completions hívás
  LLM-->>AI: Generált válasz szöveg
  AI-->>API: Válasz szöveg
  API->>API: ChatAnswer összeállítása forrásokkal
  ```

```mermaid
sequenceDiagram
Title 5. Válasz a kliensnek
  participant API as RagController
  participant SVC as RagService
  participant UI as Kliens

  API->>SVC: AskAsync kérdés
  SVC-->>API: ChatAnswer válasz + források
  API-->>UI: 200 OK ChatAnswer

```
