-- RoomBooking demo data seed
-- Adds 5 rooms and 30 realistic bookings.
-- Safe to run more than once without duplicating the demo data.

BEGIN;

INSERT INTO "Rooms" ("Name", "Capacity")
SELECT v."Name", v."Capacity"
FROM (
    VALUES
        ('Board Room', 12),
        ('Workshop Room', 10),
        ('Training Room', 16),
        ('Small Meeting Room', 4),
        ('Project Room', 8)
) AS v("Name", "Capacity")
WHERE NOT EXISTS (
    SELECT 1
    FROM "Rooms" AS r
    WHERE r."Name" = v."Name"
);

WITH demo_bookings("RoomName","User","Start","End","Attendees") AS (
    VALUES
        ('Meeting Room','alice',TIMESTAMPTZ '2026-10-12 08:00:00+00',TIMESTAMPTZ '2026-10-12 09:30:00+00',4),
        ('Conference Room','bob',TIMESTAMPTZ '2026-10-12 09:00:00+00',TIMESTAMPTZ '2026-10-12 11:00:00+00',12),
        ('Focus Room','charlie',TIMESTAMPTZ '2026-10-12 10:00:00+00',TIMESTAMPTZ '2026-10-12 11:00:00+00',2),

        ('Meeting Room','diana',TIMESTAMPTZ '2026-10-13 10:00:00+00',TIMESTAMPTZ '2026-10-13 12:00:00+00',5),
        ('Board Room','alice',TIMESTAMPTZ '2026-10-13 08:30:00+00',TIMESTAMPTZ '2026-10-13 10:30:00+00',8),
        ('Workshop Room','eric',TIMESTAMPTZ '2026-10-13 13:00:00+00',TIMESTAMPTZ '2026-10-13 15:00:00+00',7),

        ('Conference Room','frank',TIMESTAMPTZ '2026-10-14 08:00:00+00',TIMESTAMPTZ '2026-10-14 10:00:00+00',15),
        ('Training Room','bob',TIMESTAMPTZ '2026-10-14 10:30:00+00',TIMESTAMPTZ '2026-10-14 13:30:00+00',14),
        ('Small Meeting Room','charlie',TIMESTAMPTZ '2026-10-14 15:00:00+00',TIMESTAMPTZ '2026-10-14 16:00:00+00',3),

        ('Project Room','diana',TIMESTAMPTZ '2026-10-15 08:00:00+00',TIMESTAMPTZ '2026-10-15 10:00:00+00',6),
        ('Meeting Room','eric',TIMESTAMPTZ '2026-10-15 11:00:00+00',TIMESTAMPTZ '2026-10-15 12:30:00+00',6),
        ('Focus Room','frank',TIMESTAMPTZ '2026-10-15 14:00:00+00',TIMESTAMPTZ '2026-10-15 15:00:00+00',1),

        ('Board Room','bob',TIMESTAMPTZ '2026-10-16 09:00:00+00',TIMESTAMPTZ '2026-10-16 11:00:00+00',10),
        ('Workshop Room','alice',TIMESTAMPTZ '2026-10-16 11:30:00+00',TIMESTAMPTZ '2026-10-16 14:00:00+00',8),
        ('Conference Room','charlie',TIMESTAMPTZ '2026-10-16 15:00:00+00',TIMESTAMPTZ '2026-10-16 17:00:00+00',18),

        ('Training Room','diana',TIMESTAMPTZ '2026-10-19 08:30:00+00',TIMESTAMPTZ '2026-10-19 11:30:00+00',12),
        ('Small Meeting Room','eric',TIMESTAMPTZ '2026-10-19 12:00:00+00',TIMESTAMPTZ '2026-10-19 13:00:00+00',4),
        ('Project Room','frank',TIMESTAMPTZ '2026-10-19 14:00:00+00',TIMESTAMPTZ '2026-10-19 16:00:00+00',7),

        ('Meeting Room','bob',TIMESTAMPTZ '2026-10-20 08:00:00+00',TIMESTAMPTZ '2026-10-20 10:00:00+00',5),
        ('Board Room','charlie',TIMESTAMPTZ '2026-10-20 10:30:00+00',TIMESTAMPTZ '2026-10-20 12:30:00+00',9),
        ('Focus Room','alice',TIMESTAMPTZ '2026-10-20 16:00:00+00',TIMESTAMPTZ '2026-10-20 17:00:00+00',2),

        ('Conference Room','diana',TIMESTAMPTZ '2026-10-21 09:00:00+00',TIMESTAMPTZ '2026-10-21 12:00:00+00',16),
        ('Workshop Room','frank',TIMESTAMPTZ '2026-10-21 13:00:00+00',TIMESTAMPTZ '2026-10-21 15:00:00+00',9),
        ('Training Room','eric',TIMESTAMPTZ '2026-10-21 15:30:00+00',TIMESTAMPTZ '2026-10-21 18:30:00+00',15),

        ('Small Meeting Room','alice',TIMESTAMPTZ '2026-10-22 08:30:00+00',TIMESTAMPTZ '2026-10-22 09:30:00+00',2),
        ('Project Room','bob',TIMESTAMPTZ '2026-10-22 10:00:00+00',TIMESTAMPTZ '2026-10-22 12:00:00+00',8),
        ('Meeting Room','charlie',TIMESTAMPTZ '2026-10-22 14:00:00+00',TIMESTAMPTZ '2026-10-22 16:00:00+00',6),

        ('Board Room','diana',TIMESTAMPTZ '2026-10-23 08:00:00+00',TIMESTAMPTZ '2026-10-23 10:00:00+00',11),
        ('Conference Room','eric',TIMESTAMPTZ '2026-10-23 11:00:00+00',TIMESTAMPTZ '2026-10-23 14:00:00+00',20),
        ('Focus Room','frank',TIMESTAMPTZ '2026-10-23 16:00:00+00',TIMESTAMPTZ '2026-10-23 17:00:00+00',2)
)
INSERT INTO "Bookings" ("RoomId","User","Start","End","Attendees")
SELECT r."Id", d."User", d."Start", d."End", d."Attendees"
FROM demo_bookings d
JOIN "Rooms" r ON r."Name" = d."RoomName"
WHERE NOT EXISTS (
    SELECT 1
    FROM "Bookings" b
    WHERE b."RoomId" = r."Id"
      AND b."User" = d."User"
      AND b."Start" = d."Start"
      AND b."End" = d."End"
);

COMMIT;
