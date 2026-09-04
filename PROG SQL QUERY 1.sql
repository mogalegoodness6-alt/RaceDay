CREATE TABLE Users
(
UserID INT IDENTITY(1,1) PRIMARY KEY,
FirstName NVARCHAR(50) NOT NULL,
LastName NVARCHAR(50) NOT NULL,
Email NVARCHAR(100) NOT NULL UNIQUE,
PasswordHash NVARCHAR(255) NOT NULL,
Phone NVARCHAR(20) NULL,
Role NVARCHAR(20) NOT NULL
CONSTRAINT CK_Users_Role
CHECK (Role IN ('Organiser', 'Participant'))
);


CREATE TABLE Events
(
EventID INT IDENTITY(1,1) PRIMARY KEY,
OrganizerID INT NOT NULL,
EventName NVARCHAR(100) NOT NULL,
EventDate DATE NOT NULL,
Location NVARCHAR(150) NOT NULL,
Status NVARCHAR(20) NOT NULL
CONSTRAINT DF_Events_Status DEFAULT 'Upcoming',

CONSTRAINT FK_Events_Users
FOREIGN KEY (OrganizerID)
REFERENCES Users(UserID)
);


CREATE TABLE Routes
(
RouteID INT IDENTITY(1,1) PRIMARY KEY,
EventID INT NOT NULL,
RouteName NVARCHAR(100) NOT NULL,
DistanceKM DECIMAL(6,2) NOT NULL,

CONSTRAINT CK_Routes_Distance
CHECK (DistanceKM > 0),

CONSTRAINT FK_Routes_Events
FOREIGN KEY (EventID)
REFERENCES Events(EventID)
);


CREATE TABLE Weather
(
WeatherID INT IDENTITY(1,1) PRIMARY KEY,
EventID INT NOT NULL,
ForecastDate DATE NOT NULL,
Temp DECIMAL(5,2) NOT NULL,

CONSTRAINT FK_Weather_Events
FOREIGN KEY (EventID)
REFERENCES Events(EventID)
);


CREATE TABLE Categories
(
CategoryID INT IDENTITY(1,1) PRIMARY KEY,
CategoryName NVARCHAR(100) NOT NULL,
DistanceKM DECIMAL(6,2) NOT NULL,
ActivityType NVARCHAR(50) NOT NULL,

CONSTRAINT CK_Categories_Distance
CHECK (DistanceKM > 0)
);

CREATE TABLE EventCategory
(
EventID INT NOT NULL,
CategoryID INT NOT NULL,

CONSTRAINT PK_EventCategory
PRIMARY KEY (EventID, CategoryID),

CONSTRAINT FK_EventCategory_Event
FOREIGN KEY (EventID)
REFERENCES Events(EventID),

CONSTRAINT FK_EventCategory_Category
FOREIGN KEY (CategoryID)
REFERENCES Categories(CategoryID)
);


CREATE TABLE Enrollments
(
EnrollmentID INT IDENTITY(1,1) PRIMARY KEY,
EventID INT NOT NULL,
ParticipantID INT NOT NULL,
EnrollmentDate DATE NOT NULL
CONSTRAINT DF_Enrollments_EnrollmentDate DEFAULT CAST(GETDATE() AS DATE),
RaceNumber INT NOT NULL UNIQUE,

CONSTRAINT FK_Enrollments_Event
FOREIGN KEY (EventID)
REFERENCES Events(EventID),

CONSTRAINT FK_Enrollments_Participant
FOREIGN KEY (ParticipantID)
REFERENCES Users(UserID),

CONSTRAINT CK_Enrollments_RaceNumber
CHECK (RaceNumber > 0)
);

CREATE TABLE Results
(
ResultID INT IDENTITY(1,1) PRIMARY KEY,
EnrollmentID INT NOT NULL,
FinishTime TIME NULL,
Position INT NULL,
ResultStatus NVARCHAR(20) NOT NULL
CONSTRAINT DF_Results_ResultStatus DEFAULT 'Pending',

CONSTRAINT FK_Results_Enrollments
FOREIGN KEY (EnrollmentID)
REFERENCES Enrollments(EnrollmentID),

CONSTRAINT CK_Results_Position
CHECK (Position IS NULL OR Position > 0),

CONSTRAINT CK_Results_Status
CHECK (ResultStatus IN ('Pending', 'Completed', 'Disqualified'))
);

INSERT INTO Users
(FirstName, LastName, Email, PasswordHash, Phone, Role)
VALUES
('Thabo', 'Mokoena', 'thabo.mokoena@raceday.co.za',
'HASH_Thabo123', '0712345678', 'Organiser'),

('Lerato', 'Molefe', 'lerato.molefe@raceday.co.za',
'HASH_Lerato123', '0723456789', 'Organiser'),

('Kabelo', 'Dlamini', 'kabelo.dlamini@example.com',
'HASH_Kabelo123', '0734567890', 'Participant'),

('Naledi', 'Maseko', 'naledi.maseko@example.com',
'HASH_Naledi123', '0745678901', 'Participant');

SELECT *
FROM Users;

INSERT INTO Events
(OrganizerID, EventName, EventDate, Location, Status)
VALUES
(1, 'Pretoria City Marathon', '2026-10-18',
'Pretoria, Gauteng', 'Upcoming'),

(2, 'Johannesburg Spring Run', '2026-11-08',
'Johannesburg, Gauteng', 'Upcoming'),

(1, 'Soweto Community Race', '2026-12-06',
'Soweto, Gauteng', 'Upcoming');

SELECT *
FROM Events;

INSERT INTO Categories
(CategoryName, DistanceKM, ActivityType)
VALUES
('5KM Fun Run', 5.00, 'Running'),
('10KM Road Race', 10.00, 'Running'),
('21KM Half Marathon', 21.10, 'Running'),
('42KM Marathon', 42.20, 'Running');

SELECT *
FROM Categories;


INSERT INTO EventCategory
(EventID, CategoryID)
VALUES
-- Pretoria City Marathon
(1, 2),
(1, 3),
(1, 4),

-- Johannesburg Spring Run
(2, 1),
(2, 2),

-- Soweto Community Race
(3, 1),
(3, 2),
(3, 3);




SELECT
e.EventName,
c.CategoryName,
c.DistanceKM,
c.ActivityType
FROM EventCategory ec
INNER JOIN Events e
ON ec.EventID = e.EventID
INNER JOIN Categories c
ON ec.CategoryID = c.CategoryID
ORDER BY e.EventID, c.DistanceKM;

INSERT INTO Routes
(EventID, RouteName, DistanceKM)
VALUES
(1, 'Pretoria City 10KM Route', 10.00),
(1, 'Pretoria City Half Marathon Route', 21.10),
(1, 'Pretoria City Marathon Route', 42.20),

(2, 'Johannesburg Spring 5KM Route', 5.00),
(2, 'Johannesburg Spring 10KM Route', 10.00),

(3, 'Soweto Community 5KM Route', 5.00),
(3, 'Soweto Community 10KM Route', 10.00),
(3, 'Soweto Community Half Marathon Route', 21.10);


SELECT *
FROM Routes;

INSERT INTO Weather
(EventID, ForecastDate, Temp)
VALUES
(1, '2026-10-18', 22.50),
(2, '2026-11-08', 24.00),
(3, '2026-12-06', 26.50);


SELECT *
FROM Weather;

INSERT INTO Enrollments
(EventID, ParticipantID, EnrollmentDate, RaceNumber)
VALUES
(1, 3, '2026-09-01', 1001),
(1, 4, '2026-09-02', 1002),
(2, 3, '2026-09-03', 2001),
(2, 4, '2026-09-03', 2002),
(3, 3, '2026-09-04', 3001),
(3, 4, '2026-09-04', 3002);

SELECT
en.EnrollmentID,
u.FirstName + ' ' + u.LastName AS Participant,
e.EventName,
en.EnrollmentDate,
en.RaceNumber
FROM Enrollments en
INNER JOIN Users u
ON en.ParticipantID = u.UserID
INNER JOIN Events e
ON en.EventID = e.EventID
ORDER BY en.EnrollmentID;


INSERT INTO Results
(EnrollmentID, FinishTime, Position, ResultStatus)
VALUES
(1, '01:02:35', 1, 'Completed'),
(2, '01:08:42', 2, 'Completed'),
(3, '00:48:21', 1, 'Completed'),
(4, '00:52:16', 2, 'Completed'),
(5, '00:25:43', 1, 'Completed'),
(6, '00:28:57', 2, 'Completed');



SELECT
r.ResultID,
u.FirstName + ' ' + u.LastName AS Participant,
e.EventName,
en.RaceNumber,
r.FinishTime,
r.Position,
r.ResultStatus
FROM Results r
INNER JOIN Enrollments en
ON r.EnrollmentID = en.EnrollmentID
INNER JOIN Users u
ON en.ParticipantID = u.UserID
INNER JOIN Events e
ON en.EventID = e.EventID
ORDER BY e.EventID, r.Position;


