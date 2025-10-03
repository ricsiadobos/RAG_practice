```mermaid
flowchart TD
  Dev[Fejlesztõ] -->|Entitások létrehozása| EFCore[EF Core]
  EFCore -->|migrations add| Migráció
  Migráció -->|database update| SQL[(MS SQL DB)]
  SQL -->|Táblák létrejöttek| Dev