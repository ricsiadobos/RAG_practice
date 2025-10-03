```mermaid
flowchart TD
    A[Entitások megírása] --> B[migrations add InitialCreate]
    B -->|Generálja a C# migrációs fájlokat| M[Migration files]
    M --> C[database update]
    C -->|Létrejön az adatbázis| SQL[(RAGPracticeDb)]
    