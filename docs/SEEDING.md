# Demo database seed

This folder contains a small, realistic dataset for the RoomBooking PostgreSQL database.

## Files

- `seed.sql` — adds 5 rooms and 30 bookings.
- `seed-verify.sql` — checks counts, prints the data, and verifies that no bookings overlap.
- `seed-cleanup.sql` — removes only the demo data created by `seed.sql`.

The seed is idempotent: running it again does not duplicate the demo rooms or matching demo bookings.

## Seed the database

```powershell
Get-Content .\docs\seed.sql |
    docker compose exec -T postgres psql -U roombooking -d roombooking
```

## Verify the result

```powershell
Get-Content .\docs\seed-verify.sql |
    docker compose exec -T postgres psql -U roombooking -d roombooking
```

Expected result on a clean local database:

- 8 rooms
- 30 bookings
- 0 overlapping booking pairs

## Remove the demo data

```powershell
Get-Content .\docs\seed-cleanup.sql |
    docker compose exec -T postgres psql -U roombooking -d roombooking
```

The demo bookings respect the current domain rules: weekdays only, 08:00–20:00, maximum duration 4 hours, capacity limits, and no room overlaps.
