```mermaid
flowchart TD
    A[Entit�sok meg�r�sa] --> B[migrations add InitialCreate]
    B -->|Gener�lja a C# migr�ci�s f�jlokat| M[Migration files]
    M --> C[database update]
    C -->|L�trej�n az adatb�zis| SQL[(RAGPracticeDb)]
    