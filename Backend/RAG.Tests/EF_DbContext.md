```mermaid
flowchart TD
  Dev[Fejleszt�] -->|Entit�sok l�trehoz�sa| EFCore[EF Core]
  EFCore -->|migrations add| Migr�ci�
  Migr�ci� -->|database update| SQL[(MS SQL DB)]
  SQL -->|T�bl�k l�trej�ttek| Dev