```mermaid


flowchart TD
  UI[Frontend] --> API[Web API]
  API --> SVC[Rag Service]
  SVC --> RET[Retriever]
  RET --> SQL[(MS SQL)]
  SQL -->|chunks + vectors| RET
  SVC --> AI[Chat model]
  AI --> SVC
  SVC --> UI

  ```