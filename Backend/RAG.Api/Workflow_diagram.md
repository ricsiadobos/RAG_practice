```mermaid
sequenceDiagram
title 1. K�r�s fogad�sa
  participant User as Felhaszn�l�
  participant UI as Kliens
  participant API as RAG.Api RagController

  User->>UI: K�rd�s be�r�sa
  UI->>API: POST /api/rag/ask
  API-->>UI: 202 Accepted vagy 200 OK v�lasz majd
```

```mermaid
sequenceDiagram
title 2. Embadding k�sz�t�s, keres�s
  participant API as RagService
  participant RET as SimpleRetriever
  participant AI as IAiProvider
  participant STORE as InMemoryEmbeddingStore

  API->>RET: RetrieveAsync k�rd�s topK
  RET->>AI: EmbedAsync k�rd�s sz�veg
  AI-->>RET: K�rd�s vektor
  RET->>STORE: SearchHybridAsync k�rd�s sz�veg + vektor
  STORE-->>RET: Tal�latok scored chunks
  RET-->>API: Relev�ns chunks lista
```

```mermaid
sequenceDiagram
title 3. Prompt �ssze�ll�t�sa
  participant API as RagService
  participant R as Forr�s lista

  API->>API: Forr�sok r�vid�t�se
  API->>API: Prompt �ssze�ll�t�s k�rd�s + forr�sok
  API-->>R: Forr�shivatkoz�sok list�ja
```

```mermaid
sequenceDiagram
Title 4. V�lasz gener�l�sa a chat modellel
  participant API as RagService
  participant AI as IAiProvider
  participant LLM as Chat modell

  API->>AI: ChatAsync system szab�lyok + prompt
  AI->>LLM: Chat completions h�v�s
  LLM-->>AI: Gener�lt v�lasz sz�veg
  AI-->>API: V�lasz sz�veg
  API->>API: ChatAnswer �ssze�ll�t�sa forr�sokkal
  ```

```mermaid
sequenceDiagram
Title 5. V�lasz a kliensnek
  participant API as RagController
  participant SVC as RagService
  participant UI as Kliens

  API->>SVC: AskAsync k�rd�s
  SVC-->>API: ChatAnswer v�lasz + forr�sok
  API-->>UI: 200 OK ChatAnswer

```
