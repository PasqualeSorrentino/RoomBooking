# RoomBooking

[English](#english) | [Italiano](#italiano)

---

# English

## Overview

**RoomBooking** is a REST API built with **ASP.NET Core**, **Entity Framework Core**, and **PostgreSQL** for managing rooms and bookings.

The project was created as a practical exercise to explore the transition from Java to C# and to build a complete backend application using the .NET ecosystem.

The API currently includes room management, booking management, PostgreSQL persistence, EF Core migrations, configuration through the Options Pattern, custom request logging middleware, and automated tests.

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
- Seed data for rooms
- Dependency Injection
- Options Pattern
- Custom request logging middleware
- xUnit tests
- LINQ queries
- Async repository operations

---

## Tech Stack

- .NET 10
- C#
- ASP.NET Core Minimal APIs
- Entity Framework Core
- PostgreSQL 18
- Npgsql
- Docker
- xUnit
- Git / GitHub

---

## Project Structure

```text
RoomBooking/
│
├── RoomBooking.Api/
│   ├── DTOs/
│   ├── Middleware/
│   ├── Program.cs
│   └── appsettings.json
│
├── RoomBooking.Domain/
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
│   └── Data/
│       ├── Migrations/
│       └── RoomBookingDbContext.cs
│
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

## Database

The application uses PostgreSQL through Entity Framework Core.

The database includes:

- `Rooms`
- `Bookings`
- `__EFMigrationsHistory`

An index is defined on:

```text
RoomId + Start
```

to support booking queries efficiently.

The initial migration also seeds three rooms.

---

## PostgreSQL with Docker

Example command used to start PostgreSQL 18:

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

During development, the PostgreSQL connection string is stored using **.NET User Secrets**.

Example:

```powershell
dotnet user-secrets set `
  "ConnectionStrings:RoomBookingDb" `
  "Host=localhost;Port=5432;Database=roombooking;Username=roombooking;Password=<YOUR_PASSWORD>" `
  --project .\RoomBooking.Api\RoomBooking.Api.csproj
```

Sensitive values are intentionally excluded from version control.

---

## Run the Application

From the repository root:

```powershell
dotnet restore
```

Then:

```powershell
dotnet build RoomBooking.slnx
```

Apply migrations:

```powershell
dotnet ef database update `
  --project .\RoomBooking.Infrastructure\RoomBooking.Infrastructure.csproj `
  --startup-project .\RoomBooking.Api\RoomBooking.Api.csproj
```

Run the API:

```powershell
dotnet run --project .\RoomBooking.Api\RoomBooking.Api.csproj
```

The API is currently configured locally on:

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
- pricing logic.

---

## Example Requests

Create a room:

```powershell
curl `
  -Method POST `
  -Uri http://localhost:5102/rooms `
  -ContentType "application/json" `
  -Body '{
    "name":"Training Room",
    "capacity":12
  }'
```

Create a booking:

```powershell
curl `
  -Method POST `
  -Uri http://localhost:5102/bookings `
  -ContentType "application/json" `
  -Body '{
    "roomId":1,
    "user":"John Doe",
    "start":"2026-10-12T10:00:00",
    "end":"2026-10-12T12:00:00",
    "attendees":4
  }'
```

---

## Development Notes

This project was developed incrementally using a test-driven approach.

The domain was initially implemented and tested independently, then exposed through ASP.NET Core Minimal APIs and later connected to PostgreSQL using Entity Framework Core.

---

# Italiano

## Panoramica

**RoomBooking** è una REST API sviluppata con **ASP.NET Core**, **Entity Framework Core** e **PostgreSQL** per la gestione di sale e prenotazioni.

Il progetto è stato realizzato come esercizio pratico per approfondire il passaggio da Java a C# e costruire un backend completo utilizzando l'ecosistema .NET.

L'API comprende attualmente la gestione delle sale, delle prenotazioni, la persistenza su PostgreSQL, le migration di EF Core, la configurazione tramite Options Pattern, un middleware personalizzato per il logging delle richieste e test automatizzati.

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
- Dependency Injection
- Options Pattern
- Middleware personalizzato per il logging
- Test con xUnit
- Query LINQ
- Repository asincrono

---

## Tecnologie utilizzate

- .NET 10
- C#
- ASP.NET Core Minimal APIs
- Entity Framework Core
- PostgreSQL 18
- Npgsql
- Docker
- xUnit
- Git / GitHub

---

## Struttura del progetto

```text
RoomBooking/
│
├── RoomBooking.Api/
│   ├── DTOs/
│   ├── Middleware/
│   ├── Program.cs
│   └── appsettings.json
│
├── RoomBooking.Domain/
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
│   └── Data/
│       ├── Migrations/
│       └── RoomBookingDbContext.cs
│
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

## Database

L'applicazione utilizza PostgreSQL tramite Entity Framework Core.

Il database contiene:

- `Rooms`
- `Bookings`
- `__EFMigrationsHistory`

È inoltre presente un indice composto su:

```text
RoomId + Start
```

per rendere più efficienti le query sulle prenotazioni.

La migration iniziale inserisce anche tre sale di esempio.

---

## PostgreSQL con Docker

Esempio di comando per avviare PostgreSQL 18:

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

Durante lo sviluppo la connection string PostgreSQL viene salvata tramite **.NET User Secrets**.

Esempio:

```powershell
dotnet user-secrets set `
  "ConnectionStrings:RoomBookingDb" `
  "Host=localhost;Port=5432;Database=roombooking;Username=roombooking;Password=<YOUR_PASSWORD>" `
  --project .\RoomBooking.Api\RoomBooking.Api.csproj
```

I valori sensibili sono esclusi dal versionamento Git.

---

## Avvio del progetto

Dalla root del repository:

```powershell
dotnet restore
```

Poi:

```powershell
dotnet build RoomBooking.slnx
```

Applica le migration:

```powershell
dotnet ef database update `
  --project .\RoomBooking.Infrastructure\RoomBooking.Infrastructure.csproj `
  --startup-project .\RoomBooking.Api\RoomBooking.Api.csproj
```

Avvia l'API:

```powershell
dotnet run --project .\RoomBooking.Api\RoomBooking.Api.csproj
```

L'API è attualmente configurata localmente su:

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
- logica delle tariffe.

---

## Esempi di richiesta

Creazione di una sala:

```powershell
curl `
  -Method POST `
  -Uri http://localhost:5102/rooms `
  -ContentType "application/json" `
  -Body '{
    "name":"Training Room",
    "capacity":12
  }'
```

Creazione di una prenotazione:

```powershell
curl `
  -Method POST `
  -Uri http://localhost:5102/bookings `
  -ContentType "application/json" `
  -Body '{
    "roomId":1,
    "user":"Mario Rossi",
    "start":"2026-10-12T10:00:00",
    "end":"2026-10-12T12:00:00",
    "attendees":4
  }'
```

---

## Note di sviluppo

Il progetto è stato sviluppato in modo incrementale seguendo un approccio orientato ai test.

Il dominio è stato inizialmente implementato e testato in modo indipendente, successivamente esposto tramite ASP.NET Core Minimal APIs e infine collegato a PostgreSQL attraverso Entity Framework Core.