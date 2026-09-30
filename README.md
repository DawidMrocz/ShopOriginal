# ShopOriginal

Przykładowy **sklep internetowy** zbudowany jako zestaw mikroserwisów .NET z panelem back office
i frontendem w Vue/Quasar. Projekt szkoleniowy.

## Architektura

| Projekt | Rola |
|---|---|
| `UserApp` | Rejestracja, logowanie i zarządzanie użytkownikami |
| `ProductApp`, `ProductAppGrpc` | Katalog produktów (REST oraz gRPC) |
| `BasketApp` | Koszyk zakupowy |
| `BackOfficeUserApp`, `BackOfficeProductApp`, `BackOfficeDiscountApp` | Panel administracyjny: użytkownicy, produkty, rabaty |
| `Contracts`, `BackOfficeContracts` | Kontrakty i wiadomości współdzielone między usługami |
| `Framework` | Wspólna biblioteka: uwierzytelnianie, walidacja, filtry, serwisy |
| `ServiceBroker`, `Jobs` | Przetwarzanie w tle (SQL Service Broker, zadania cykliczne) |
| `WWW` | Frontend: Vue 3 + Quasar |

## Technologie

.NET 8, ASP.NET Core, Entity Framework Core, SQL Server, gRPC, RabbitMQ, Docker Compose, Vue 3 + Quasar.

## Uruchomienie

1. Utwórz lokalnie pliki `appsettings.json` w usługach (są w `.gitignore`): connection string do SQL Server,
   ustawienia RabbitMQ i JWT.
2. Wygeneruj własny certyfikat deweloperski (`*.pfx` nie jest w repozytorium) i ustaw jego hasło
   w `docker-compose.yml` (`YOUR_CERT_PASSWORD`).
3. `docker compose up --build`
4. Frontend: `cd WWW && npm install && npx quasar dev`

## Status

Projekt edukacyjny.
