-- Benchmark: EXPLAIN ANALYZE before and after the composite index
--
-- Synthetic data is inserted inside a transaction so that
-- the database is restored to its original state at the end.

BEGIN;

-- Insert 10,000 synthetic bookings for the benchmark.

INSERT INTO "Bookings"
(
    "RoomId",
    "User",
    "Start",
    "End",
    "Attendees"
)
SELECT
    ((g - 1) % 3) + 1,
    'BenchmarkUser' || g,
    TIMESTAMPTZ '2026-01-01 08:00:00+00'
        + ((g % 240) * INTERVAL '1 day')
        + ((g % 10) * INTERVAL '1 hour'),
    TIMESTAMPTZ '2026-01-01 09:00:00+00'
        + ((g % 240) * INTERVAL '1 day')
        + ((g % 10) * INTERVAL '1 hour'),
    ((g % 5) + 1)
FROM generate_series(1, 10000) AS g;


-- ============================================================
-- WITHOUT INDEX
-- ============================================================

DROP INDEX IF EXISTS "IX_Bookings_RoomId_Start";

EXPLAIN (ANALYZE, BUFFERS)
SELECT
    b."Id",
    b."RoomId",
    b."User",
    b."Start",
    b."End",
    b."Attendees"
FROM "Bookings" AS b
WHERE b."RoomId" = 1
  AND b."Start" >= TIMESTAMPTZ '2026-03-01 00:00:00+00'
  AND b."Start" < TIMESTAMPTZ '2026-03-08 00:00:00+00';


-- ============================================================
-- WITH INDEX
-- ============================================================

CREATE INDEX "IX_Bookings_RoomId_Start"
ON "Bookings" ("RoomId", "Start");

EXPLAIN (ANALYZE, BUFFERS)
SELECT
    b."Id",
    b."RoomId",
    b."User",
    b."Start",
    b."End",
    b."Attendees"
FROM "Bookings" AS b
WHERE b."RoomId" = 1
  AND b."Start" >= TIMESTAMPTZ '2026-03-01 00:00:00+00'
  AND b."Start" < TIMESTAMPTZ '2026-03-08 00:00:00+00';

ROLLBACK;