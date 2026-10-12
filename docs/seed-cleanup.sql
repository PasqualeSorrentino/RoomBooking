-- Remove only demo data created by seed.sql.
-- The original 3 EF Core seeded rooms are preserved.

BEGIN;

DELETE FROM "Bookings"
WHERE "User" IN ('alice','bob','charlie','diana','eric','frank')
  AND "Start" >= TIMESTAMPTZ '2026-10-12 00:00:00+00'
  AND "Start" <  TIMESTAMPTZ '2026-10-24 00:00:00+00';

DELETE FROM "Rooms"
WHERE "Name" IN (
    'Board Room',
    'Workshop Room',
    'Training Room',
    'Small Meeting Room',
    'Project Room'
)
AND NOT EXISTS (
    SELECT 1
    FROM "Bookings" b
    WHERE b."RoomId" = "Rooms"."Id"
);

COMMIT;
