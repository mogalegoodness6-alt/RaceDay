--TESTING

SELECT 'Users' AS TableName, COUNT(*) AS RecordCount FROM Users
UNION ALL
SELECT 'Events', COUNT(*) FROM Events
UNION ALL
SELECT 'Routes', COUNT(*) FROM Routes
UNION ALL
SELECT 'Weather', COUNT(*) FROM Weather
UNION ALL
SELECT 'Categories', COUNT(*) FROM Categories
UNION ALL
SELECT 'EventCategory', COUNT(*) FROM EventCategory
UNION ALL
SELECT 'Enrollments', COUNT(*) FROM Enrollments
UNION ALL
SELECT 'Results', COUNT(*) FROM Results;

SELECT
e.EventName,
u.FirstName + ' ' + u.LastName AS Organiser,
r.RouteName,
r.DistanceKM
FROM Events e
INNER JOIN Users u
ON e.OrganizerID = u.UserID
INNER JOIN Routes r
ON e.EventID = r.EventID
ORDER BY e.EventID, r.DistanceKM;

SELECT
u.FirstName + ' ' + u.LastName AS Participant,
e.EventName,
en.RaceNumber,
r.FinishTime,
r.Position,
r.ResultStatus
FROM Enrollments en
INNER JOIN Users u
ON en.ParticipantID = u.UserID
INNER JOIN Events e
ON en.EventID = e.EventID
INNER JOIN Results r
ON en.EnrollmentID = r.EnrollmentID
ORDER BY e.EventID, r.Position;
