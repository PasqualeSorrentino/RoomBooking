# RoomBooking

[English](#english) | [Italiano](#italiano)

---

# English

## Overview

**RoomBooking** is a REST API built with **ASP.NET Core**, **Entity Framework Core**, and **PostgreSQL** for managing rooms and bookings.

The project was created as a practical exercise to explore the transition from Java to C# and to build a complete backend application using the .NET ecosystem.

The API currently includes room and booking management, PostgreSQL persistence, EF Core migrations, structured error handling with Problem Details, OpenAPI documentation, Scalar UI, automated unit and integration tests, Docker support, reusable SQL analysis scripts, and demo seed data.

---

## Features

- CRUD operations for rooms
- Create and delete bookings
- Retrieve bookings for a specific room
- Booking validation rules
- Room capacity validation
- Booking overlap prevention
- Weekday-only bookings
- Opening and closing time constraints
- Maximum booking duration
- PostgreSQL persistence
- Entity Framework Core migrations
- Initial room seed data
- Demo database seed scripts
- Dependency Injection
- Options Pattern
- Custom request logging middleware
- Centralized exception handling
- RFC 7807-style Problem Details responses
- OpenAPI documentation
- Scalar API reference UI
- xUnit unit tests
- ASP.NET Core integration tests with `WebApplicationFactory`
- LINQ queries
- Async repository operations
- Docker multi-stage build
- Docker Compose stack with API + PostgreSQL
- SQL analysis and `EXPLAIN ANALYZE` performance benchmark

---

## Tech Stack

- .NET 10
- C#
- ASP.NET Core Minimal APIs
- Entity Framework Core
- PostgreSQL 18
- Npgsql
- Docker
- Docker Compose
- xUnit
- `Microsoft.AspNetCore.Mvc.Testing`
- EF Core InMemory provider for integration tests
- Scalar
- Git / GitHub

---

## Project Structure

```text
RoomBooking/
│
├── RoomBooking.Api/
│   ├── DTOs/
│   ├── Errors/
│   │   └── GlobalExceptionHandler.cs
│   ├── Middleware/
│   ├── Program.cs
│   └── appsettings.json
│
├── RoomBooking.Api.Tests/
│   ├── BookingEndpointsTests.cs
│   ├── CustomWebApplicationFactory.cs
│   ├── HealthTests.cs
│   └── RoomEndpointsTests.cs
│
├── RoomBooking.Domain/
│   ├── Exceptions/
│   ├── Booking.cs
│   ├── BookingOptions.cs
│   ├── BookingService.cs
│   ├── IBookingRepository.cs
│   ├── InMemoryBookingRepository.cs
│   ├── PricingService.cs
│   └── Room.cs
│
├── RoomBooking.Domain.Tests/
│   ├── BookingServiceTests.cs
│   ├── BookingTests.cs
│   ├── InMemoryBookingRepositoryTests.cs
│   ├── LinqDeferredExecutionTests.cs
│   ├── PricingServiceTests.cs
│   └── RoomTest.cs
│
├── RoomBooking.Infrastructure/
│   ├── Data/
│   │   ├── Migrations/
│   │   └── RoomBookingDbContext.cs
│   └── Repositories/
│       └── EfBookingRepository.cs
│
├── docs/
│   ├── queries.sql
│   ├── explain.sql
│   ├── seed.sql
│   ├── seed-verify.sql
│   ├── seed-cleanup.sql
│   └── SEEDING.md
│
├── Dockerfile
├── docker-compose.yml
├── .dockerignore
├── .env.example
├── RoomBooking.slnx
└── README.md
```

---

## API Endpoints

### Rooms

| Method | Endpoint | Description |
|---|---|---|
| GET | `/rooms` | Get all rooms |
| GET | `/rooms/{id}` | Get a room by ID |
| POST | `/rooms` | Create a new room |
| PUT | `/rooms/{id}` | Update a room |
| DELETE | `/rooms/{id}` | Delete a room |
| GET | `/rooms/{id}/bookings` | Get bookings for a specific room |

### Bookings

| Method | Endpoint | Description |
|---|---|---|
| POST | `/bookings` | Create a booking |
| DELETE | `/bookings/{id}` | Delete a booking |

---

## Booking Rules

A booking must satisfy the following rules:

- the room must exist;
- the number of attendees cannot exceed the room capacity;
- bookings are allowed only on weekdays;
- bookings must be within the configured opening hours;
- a booking cannot exceed the configured maximum duration;
- two bookings for the same room cannot overlap.

Default configuration:

```json
"Booking": {
  "OpeningHour": 8,
  "ClosingHour": 20,
  "MaxDurationHours": 4
}
```

---

## Error Handling

The API uses centralized exception handling through `IExceptionHandler` and returns Problem Details responses.

Main mappings:

| Scenario | HTTP Status |
|---|---:|
| Invalid booking data | `400 Bad Request` |
| Malformed request body | `400 Bad Request` |
| Room not found | `404 Not Found` |
| Booking overlap | `409 Conflict` |
| Unexpected error | `500 Internal Server Error` |

This keeps endpoint code focused on application logic while error responses remain consistent across the API.

---

## OpenAPI and Scalar

OpenAPI is enabled in the Development environment.

OpenAPI document:

```text
http://localhost:5102/openapi/v1.json
```

Scalar UI:

```text
http://localhost:5102/scalar/v1
```

Endpoints include summaries, descriptions, success response metadata, and documented problem response codes.

---

## Database

The application uses PostgreSQL through Entity Framework Core.

The database includes:

- `Rooms`
- `Bookings`
- `__EFMigrationsHistory`

A composite index is defined on:

```text
RoomId + Start
```

to support booking queries efficiently.

The initial migration seeds three rooms.

The application applies EF Core migrations automatically at startup when a relational provider is in use. During integration tests, the InMemory provider uses `EnsureCreated()` instead.

---

## Demo Database Seed

Additional demo data is available under `docs/`.

On a clean database, the demo seed adds:

- 5 additional rooms;
- 30 realistic bookings;
- 6 demo users;
- bookings without room overlaps and compatible with the current domain rules.

Populate the database:

```powershell
Get-Content .\docs\seed.sql |
    docker compose exec -T postgres psql -U roombooking -d roombooking
```

Verify the data:

```powershell
Get-Content .\docs\seed-verify.sql |
    docker compose exec -T postgres psql -U roombooking -d roombooking
```

Remove only the demo data:

```powershell
Get-Content .\docs\seed-cleanup.sql |
    docker compose exec -T postgres psql -U roombooking -d roombooking
```

More details are available in `docs/SEEDING.md`.

---

## SQL Analysis

The repository includes reusable SQL exercises and performance analysis scripts.

`docs/queries.sql` includes queries for:

- free rooms in a given time interval;
- room occupancy percentage;
- overlapping bookings through a self-join;
- top users by total booked hours.

`docs/explain.sql` contains a repeatable `EXPLAIN (ANALYZE, BUFFERS)` benchmark that compares the same booking query before and after the composite index on:

```text
(RoomId, Start)
```

The benchmark uses temporary synthetic data inside a transaction and rolls back the changes at the end.

The PostgreSQL exclusion-constraint approach with `tstzrange` was also explored to prevent overlapping bookings directly at database level.

---

## Docker

The API uses a multi-stage Docker build.

Build the API image:

```powershell
docker build -t roombooking-api .
```

The project also includes `docker-compose.yml` with:

- the ASP.NET Core API;
- PostgreSQL 18;
- a persistent PostgreSQL volume;
- a database health check;
- service dependency management.

Create a local `.env` file from `.env.example` and provide a development password:

```text
POSTGRES_PASSWORD=your-local-password
```

The real `.env` file is excluded from version control.

Start the complete stack:

```powershell
docker compose up --build
```

Run in background:

```powershell
docker compose up -d --build
```

Stop the stack:

```powershell
docker compose down
```

The API is exposed locally on:

```text
http://localhost:5102
```

---

## PostgreSQL without Docker Compose

PostgreSQL can also be started independently.

Example:

```powershell
docker run --name roombooking-postgres `
  -e POSTGRES_USER=roombooking `
  -e POSTGRES_PASSWORD=<YOUR_PASSWORD> `
  -e POSTGRES_DB=roombooking `
  -p 5432:5432 `
  -v roombooking-postgres-data:/var/lib/postgresql `
  -d postgres:18
```

Do not store real credentials inside the repository.

---

## Configuration and Secrets

During local development, the PostgreSQL connection string can be stored using **.NET User Secrets**.

Example:

```powershell
dotnet user-secrets set `
  "ConnectionStrings:RoomBookingDb" `
  "Host=localhost;Port=5432;Database=roombooking;Username=roombooking;Password=<YOUR_PASSWORD>" `
  --project .\RoomBooking.Api\RoomBooking.Api.csproj
```

Sensitive values are intentionally excluded from version control.

---

## Run the Application Locally

From the repository root:

```powershell
dotnet restore
```

Then:

```powershell
dotnet build RoomBooking.slnx
```

Apply migrations manually if needed:

```powershell
dotnet ef database update `
  --project .\RoomBooking.Infrastructure\RoomBooking.Infrastructure.csproj `
  --startup-project .\RoomBooking.Api\RoomBooking.Api.csproj
```

Run the API:

```powershell
dotnet run --project .\RoomBooking.Api\RoomBooking.Api.csproj
```

The API is configured locally on:

```text
http://localhost:5102
```

---

## Run Tests

```powershell
dotnet test RoomBooking.slnx
```

The test suite covers:

- domain validation;
- booking business rules;
- async repository behavior;
- LINQ queries;
- deferred execution;
- pricing logic;
- application startup;
- room endpoint integration;
- valid booking creation;
- booking overlap conflicts;
- capacity validation;
- missing-room responses;
- booking deletion.

Integration tests use `WebApplicationFactory<Program>` and replace PostgreSQL with the EF Core InMemory provider.

---

## Example Requests

Create a room:

```powershell
$body = @{
    name = "Training Room"
    capacity = 12
} | ConvertTo-Json

Invoke-WebRequest `
    -Method POST `
    -Uri http://localhost:5102/rooms `
    -ContentType "application/json" `
    -Body $body
```

Create a booking:

```powershell
$body = @{
    roomId = 1
    user = "John Doe"
    start = "2026-10-13T10:00:00"
    end = "2026-10-13T12:00:00"
    attendees = 4
} | ConvertTo-Json

Invoke-WebRequest `
    -Method POST `
    -Uri http://localhost:5102/bookings `
    -ContentType "application/json" `
    -Body $body
```

---

## Development Notes

The project was developed incrementally using a test-oriented workflow.

The domain was initially implemented and tested independently, then exposed through ASP.NET Core Minimal APIs, connected to PostgreSQL with Entity Framework Core, extended with centralized API error handling and OpenAPI documentation, covered with integration tests, containerized with Docker, and complemented with SQL analysis and reproducible database demo data.

Development is organized through short-lived Git branches and focused commits to keep the history easy to review and revert.

---

# Italiano

## Panoramica

**RoomBooking** è una REST API sviluppata con **ASP.NET Core**, **Entity Framework Core** e **PostgreSQL** per la gestione di sale e prenotazioni.

Il progetto è stato realizzato come esercizio pratico per approfondire il passaggio da Java a C# e costruire un backend completo utilizzando l'ecosistema .NET.

L'API comprende attualmente la gestione delle sale e delle prenotazioni, la persistenza su PostgreSQL, le migration di EF Core, la gestione centralizzata degli errori tramite Problem Details, la documentazione OpenAPI, l'interfaccia Scalar, test unitari e di integrazione, supporto Docker, script SQL riutilizzabili e dati demo per il database.

---

## Funzionalità

- Operazioni CRUD sulle sale
- Creazione e cancellazione delle prenotazioni
- Recupero delle prenotazioni relative a una sala
- Validazione delle prenotazioni
- Controllo della capienza delle sale
- Prevenzione delle sovrapposizioni
- Prenotazioni consentite solo nei giorni feriali
- Vincoli sugli orari di apertura e chiusura
- Durata massima della prenotazione
- Persistenza PostgreSQL
- Migration con Entity Framework Core
- Seed iniziale delle sale
- Script di seed demo
- Dependency Injection
- Options Pattern
- Middleware personalizzato per il logging
- Gestione centralizzata delle eccezioni
- Risposte Problem Details
- Documentazione OpenAPI
- Interfaccia Scalar
- Test unitari con xUnit
- Test di integrazione con `WebApplicationFactory`
- Query LINQ
- Repository asincrono
- Docker multi-stage
- Docker Compose con API + PostgreSQL
- Query SQL di analisi
- Benchmark `EXPLAIN ANALYZE`

---

## Tecnologie utilizzate

- .NET 10
- C#
- ASP.NET Core Minimal APIs
- Entity Framework Core
- PostgreSQL 18
- Npgsql
- Docker
- Docker Compose
- xUnit
- `Microsoft.AspNetCore.Mvc.Testing`
- EF Core InMemory per i test di integrazione
- Scalar
- Git / GitHub

---

## Struttura del progetto

```text
RoomBooking/
│
├── RoomBooking.Api/
│   ├── DTOs/
│   ├── Errors/
│   │   └── GlobalExceptionHandler.cs
│   ├── Middleware/
│   ├── Program.cs
│   └── appsettings.json
│
├── RoomBooking.Api.Tests/
│   ├── BookingEndpointsTests.cs
│   ├── CustomWebApplicationFactory.cs
│   ├── HealthTests.cs
│   └── RoomEndpointsTests.cs
│
├── RoomBooking.Domain/
│   ├── Exceptions/
│   ├── Booking.cs
│   ├── BookingOptions.cs
│   ├── BookingService.cs
│   ├── IBookingRepository.cs
│   ├── InMemoryBookingRepository.cs
│   ├── PricingService.cs
│   └── Room.cs
│
├── RoomBooking.Domain.Tests/
│   ├── BookingServiceTests.cs
│   ├── BookingTests.cs
│   ├── InMemoryBookingRepositoryTests.cs
│   ├── LinqDeferredExecutionTests.cs
│   ├── PricingServiceTests.cs
│   └── RoomTest.cs
│
├── RoomBooking.Infrastructure/
│   ├── Data/
│   │   ├── Migrations/
│   │   └── RoomBookingDbContext.cs
│   └── Repositories/
│       └── EfBookingRepository.cs
│
├── docs/
│   ├── queries.sql
│   ├── explain.sql
│   ├── seed.sql
│   ├── seed-verify.sql
│   ├── seed-cleanup.sql
│   └── SEEDING.md
│
├── Dockerfile
├── docker-compose.yml
├── .dockerignore
├── .env.example
├── RoomBooking.slnx
└── README.md
```

---

## Endpoint API

### Sale

| Metodo | Endpoint | Descrizione |
|---|---|---|
| GET | `/rooms` | Restituisce tutte le sale |
| GET | `/rooms/{id}` | Restituisce una sala tramite ID |
| POST | `/rooms` | Crea una nuova sala |
| PUT | `/rooms/{id}` | Aggiorna una sala |
| DELETE | `/rooms/{id}` | Elimina una sala |
| GET | `/rooms/{id}/bookings` | Restituisce le prenotazioni di una sala |

### Prenotazioni

| Metodo | Endpoint | Descrizione |
|---|---|---|
| POST | `/bookings` | Crea una prenotazione |
| DELETE | `/bookings/{id}` | Elimina una prenotazione |

---

## Regole di prenotazione

Una prenotazione deve rispettare le seguenti regole:

- la sala deve esistere;
- il numero di partecipanti non può superare la capienza della sala;
- le prenotazioni sono consentite solo nei giorni feriali;
- gli orari devono rientrare nella fascia configurata;
- la prenotazione non può superare la durata massima configurata;
- due prenotazioni della stessa sala non possono sovrapporsi.

Configurazione predefinita:

```json
"Booking": {
  "OpeningHour": 8,
  "ClosingHour": 20,
  "MaxDurationHours": 4
}
```

---

## Gestione degli errori

L'API utilizza una gestione centralizzata delle eccezioni tramite `IExceptionHandler` e restituisce risposte Problem Details.

Mappature principali:

| Scenario | Stato HTTP |
|---|---:|
| Dati di prenotazione non validi | `400 Bad Request` |
| Body della richiesta non valido | `400 Bad Request` |
| Sala inesistente | `404 Not Found` |
| Sovrapposizione di prenotazioni | `409 Conflict` |
| Errore inatteso | `500 Internal Server Error` |

In questo modo gli endpoint possono concentrarsi sulla logica applicativa e le risposte di errore rimangono uniformi.

---

## OpenAPI e Scalar

OpenAPI è attivo nell'ambiente Development.

Documento OpenAPI:

```text
http://localhost:5102/openapi/v1.json
```

Interfaccia Scalar:

```text
http://localhost:5102/scalar/v1
```

Gli endpoint espongono summary, descrizioni, codici di successo e principali risposte Problem Details.

---

## Database

L'applicazione utilizza PostgreSQL tramite Entity Framework Core.

Il database contiene:

- `Rooms`
- `Bookings`
- `__EFMigrationsHistory`

È presente un indice composto su:

```text
RoomId + Start
```

per rendere più efficienti le query sulle prenotazioni.

La migration iniziale inserisce tre sale.

All'avvio dell'applicazione le migration EF Core vengono applicate automaticamente quando viene utilizzato un provider relazionale. Durante i test di integrazione, il provider InMemory utilizza invece `EnsureCreated()`.

---

## Dati demo del database

Nella cartella `docs/` sono disponibili script per popolare il database con dati demo.

Su un database pulito il seed aggiunge:

- 5 sale aggiuntive;
- 30 prenotazioni realistiche;
- 6 utenti demo;
- prenotazioni compatibili con le regole correnti e senza sovrapposizioni per sala.

Popolamento:

```powershell
Get-Content .\docs\seed.sql |
    docker compose exec -T postgres psql -U roombooking -d roombooking
```

Verifica:

```powershell
Get-Content .\docs\seed-verify.sql |
    docker compose exec -T postgres psql -U roombooking -d roombooking
```

Rimozione dei soli dati demo:

```powershell
Get-Content .\docs\seed-cleanup.sql |
    docker compose exec -T postgres psql -U roombooking -d roombooking
```

Ulteriori informazioni sono disponibili in `docs/SEEDING.md`.

---

## Analisi SQL

Il repository contiene query SQL riutilizzabili e script dedicati all'analisi delle performance.

`docs/queries.sql` comprende query per:

- sale libere in un determinato intervallo;
- percentuale di occupazione delle sale;
- ricerca delle prenotazioni sovrapposte tramite self-join;
- top utenti per ore complessivamente prenotate.

`docs/explain.sql` contiene un benchmark ripetibile con:

```text
EXPLAIN (ANALYZE, BUFFERS)
```

che confronta la stessa query prima e dopo l'indice composto:

```text
(RoomId, Start)
```

Il benchmark utilizza dati sintetici temporanei all'interno di una transazione e ripristina il database con `ROLLBACK`.

È stato inoltre analizzato l'uso di un exclusion constraint PostgreSQL con `tstzrange` per impedire le sovrapposizioni direttamente a livello database.

---

## Docker

L'API utilizza una build Docker multi-stage.

Build dell'immagine:

```powershell
docker build -t roombooking-api .
```

Il progetto include inoltre `docker-compose.yml` con:

- API ASP.NET Core;
- PostgreSQL 18;
- volume persistente;
- health check PostgreSQL;
- dipendenza dell'API dallo stato di salute del database.

Crea localmente un file `.env` partendo da `.env.example`:

```text
POSTGRES_PASSWORD=your-local-password
```

Il file `.env` reale è escluso dal versionamento.

Avvio dello stack:

```powershell
docker compose up --build
```

Avvio in background:

```powershell
docker compose up -d --build
```

Arresto:

```powershell
docker compose down
```

L'API viene esposta localmente su:

```text
http://localhost:5102
```

---

## PostgreSQL senza Docker Compose

È possibile avviare PostgreSQL anche separatamente.

Esempio:

```powershell
docker run --name roombooking-postgres `
  -e POSTGRES_USER=roombooking `
  -e POSTGRES_PASSWORD=<YOUR_PASSWORD> `
  -e POSTGRES_DB=roombooking `
  -p 5432:5432 `
  -v roombooking-postgres-data:/var/lib/postgresql `
  -d postgres:18
```

Le credenziali reali non devono essere salvate nel repository.

---

## Configurazione e secrets

Durante lo sviluppo locale la connection string PostgreSQL può essere salvata tramite **.NET User Secrets**.

Esempio:

```powershell
dotnet user-secrets set `
  "ConnectionStrings:RoomBookingDb" `
  "Host=localhost;Port=5432;Database=roombooking;Username=roombooking;Password=<YOUR_PASSWORD>" `
  --project .\RoomBooking.Api\RoomBooking.Api.csproj
```

I valori sensibili sono esclusi dal versionamento Git.

---

## Avvio locale del progetto

Dalla root del repository:

```powershell
dotnet restore
```

Poi:

```powershell
dotnet build RoomBooking.slnx
```

Se necessario, applica manualmente le migration:

```powershell
dotnet ef database update `
  --project .\RoomBooking.Infrastructure\RoomBooking.Infrastructure.csproj `
  --startup-project .\RoomBooking.Api\RoomBooking.Api.csproj
```

Avvia l'API:

```powershell
dotnet run --project .\RoomBooking.Api\RoomBooking.Api.csproj
```

L'API è configurata localmente su:

```text
http://localhost:5102
```

---

## Esecuzione dei test

```powershell
dotnet test RoomBooking.slnx
```

La suite di test copre:

- validazione del dominio;
- regole di business delle prenotazioni;
- repository asincrono;
- query LINQ;
- deferred execution;
- logica delle tariffe;
- avvio dell'applicazione;
- integrazione degli endpoint delle sale;
- creazione valida di una prenotazione;
- conflitti dovuti a sovrapposizioni;
- controllo della capienza;
- sala inesistente;
- cancellazione della prenotazione.

I test di integrazione utilizzano `WebApplicationFactory<Program>` e sostituiscono PostgreSQL con il provider EF Core InMemory.

---

## Esempi di richiesta

Creazione di una sala:

```powershell
$body = @{
    name = "Training Room"
    capacity = 12
} | ConvertTo-Json

Invoke-WebRequest `
    -Method POST `
    -Uri http://localhost:5102/rooms `
    -ContentType "application/json" `
    -Body $body
```

Creazione di una prenotazione:

```powershell
$body = @{
    roomId = 1
    user = "Mario Rossi"
    start = "2026-10-13T10:00:00"
    end = "2026-10-13T12:00:00"
    attendees = 4
} | ConvertTo-Json

Invoke-WebRequest `
    -Method POST `
    -Uri http://localhost:5102/bookings `
    -ContentType "application/json" `
    -Body $body
```

---

## Note di sviluppo

Il progetto è stato sviluppato in modo incrementale seguendo un approccio orientato ai test.

Il dominio è stato inizialmente implementato e testato in modo indipendente, successivamente esposto tramite ASP.NET Core Minimal APIs, collegato a PostgreSQL attraverso Entity Framework Core, esteso con gestione centralizzata degli errori e documentazione OpenAPI, coperto tramite test di integrazione, containerizzato con Docker e completato con analisi SQL e dati demo riproducibili.

Lo sviluppo viene organizzato tramite branch Git brevi e commit focalizzati, così da mantenere una cronologia semplice da revisionare e, se necessario, da ripristinare.
