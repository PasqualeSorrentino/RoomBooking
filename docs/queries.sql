-- 1. Free rooms in a given time interval
-- Example interval:
-- 2026-10-13 10:00 -> 2026-10-13 12:00

SELECT
    r."Id",
    r."Name",
    r."Capacity"
FROM "Rooms" AS r
WHERE NOT EXISTS (
    SELECT 1
    FROM "Bookings" AS b
    WHERE b."RoomId" = r."Id"
      AND b."Start" < TIMESTAMPTZ '2026-10-13 12:00:00+00'
      AND b."End" > TIMESTAMPTZ '2026-10-13 10:00:00+00'
)
ORDER BY r."Id";

-- 2. Occupancy percentage per room in the last 7 days
-- Assumption:
-- each room is available 12 hours per weekday, from 08:00 to 20:00.
-- Over 7 calendar days we calculate booked hours against 60 available hours
-- (5 working days * 12 hours).

SELECT
    r."Id",
    r."Name",
    ROUND(
        COALESCE(
            SUM(
                EXTRACT(
                    EPOCH FROM (b."End" - b."Start")
                ) / 3600.0
            ),
            0
        ),
        2
    ) AS "BookedHours",
    ROUND(
        COALESCE(
            SUM(
                EXTRACT(
                    EPOCH FROM (b."End" - b."Start")
                ) / 3600.0
            ),
            0
        ) / 60.0 * 100,
        2
    ) AS "OccupancyPercentage"
FROM "Rooms" AS r
LEFT JOIN "Bookings" AS b
    ON b."RoomId" = r."Id"
    AND b."Start" >= NOW() - INTERVAL '7 days'
    AND b."Start" < NOW()
GROUP BY
    r."Id",
    r."Name"
ORDER BY
    r."Id";

-- 3. Detect overlapping bookings with a self-join
-- Two bookings overlap when:
-- first.Start < second.End
-- AND
-- first.End > second.Start
--
-- b1.Id < b2.Id prevents comparing the same pair twice.

SELECT
    b1."Id" AS "Booking1Id",
    b2."Id" AS "Booking2Id",
    b1."RoomId",
    b1."Start" AS "Booking1Start",
    b1."End" AS "Booking1End",
    b2."Start" AS "Booking2Start",
    b2."End" AS "Booking2End"
FROM "Bookings" AS b1
JOIN "Bookings" AS b2
    ON b1."RoomId" = b2."RoomId"
    AND b1."Id" < b2."Id"
    AND b1."Start" < b2."End"
    AND b1."End" > b2."Start"
ORDER BY
    b1."RoomId",
    b1."Start";

-- 4. Top 3 users by total booked hours

SELECT
    b."User",
    COUNT(*) AS "BookingCount",
    ROUND(
        SUM(
            EXTRACT(
                EPOCH FROM (b."End" - b."Start")
            ) / 3600.0
        ),
        2
    ) AS "TotalBookedHours"
FROM "Bookings" AS b
GROUP BY
    b."User"
ORDER BY
    "TotalBookedHours" DESC
LIMIT 3;

-- ============================================================
-- Prevent overlapping bookings at database level
-- ============================================================
--
-- PostgreSQL can enforce the no-overlap rule directly
-- using an EXCLUDE constraint.
--
-- btree_gist is required to use equality (=) on RoomId
-- together with range overlap (&&).

-- CREATE EXTENSION IF NOT EXISTS btree_gist;

-- ALTER TABLE "Bookings"
-- ADD CONSTRAINT "EX_Bookings_NoOverlap"
-- EXCLUDE USING gist
-- (
--     "RoomId" WITH =,
--     tstzrange("Start", "End", '[)') WITH &&
-- );