```mermaid
sequenceDiagram
  participant UI as Kliens
  participant API as Web API
  participant SVC as Rag Service
  participant RET as Retriever
  participant DB as MS SQL
  participant AI as Chat model

  UI->>API: K�rd�s
  API->>SVC: AskAsync
  SVC->>RET: RetrieveAsync
  RET->>DB: El�sz�r�s chunks sz�vegre �s forr�sra
  DB-->>RET: Jel�ltek list�ja
  RET->>DB: Embeddingek jel�ltekhez
  DB-->>RET: Vektorok
  RET-->>SVC: Top k relev�ns chunk
  SVC->>AI: Prompt forr�sokkal
  AI-->>SVC: V�lasz
  SVC-->>API: ChatAnswer
  API-->>UI: 200 OK
```