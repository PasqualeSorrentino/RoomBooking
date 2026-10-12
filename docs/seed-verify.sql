-- Verify demo data

SELECT COUNT(*) AS "RoomCount" FROM "Rooms";
SELECT COUNT(*) AS "BookingCount" FROM "Bookings";

SELECT "Id","Name","Capacity"
FROM "Rooms"
ORDER BY "Id";

SELECT
    b."Id",
    r."Name" AS "Room",
    b."User",
    b."Start",
    b."End",
    b."Attendees"
FROM "Bookings" b
JOIN "Rooms" r ON r."Id" = b."RoomId"
ORDER BY b."Start", r."Name";

-- Should return 0 rows.
SELECT
    b1."Id" AS "Booking1Id",
    b2."Id" AS "Booking2Id",
    r."Name" AS "Room",
    b1."Start" AS "Booking1Start",
    b1."End" AS "Booking1End",
    b2."Start" AS "Booking2Start",
    b2."End" AS "Booking2End"
FROM "Bookings" b1
JOIN "Bookings" b2
  ON b1."RoomId" = b2."RoomId"
 AND b1."Id" < b2."Id"
 AND b1."Start" < b2."End"
 AND b1."End" > b2."Start"
JOIN "Rooms" r ON r."Id" = b1."RoomId"
ORDER BY r."Name", b1."Start";
