-- Create Database
CREATE DATABASE Clinical;
GO
USE Clinical;
GO

-- 1️⃣ USERS TABLE
CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Role VARCHAR(50) NOT NULL,
    Email VARCHAR(100) UNIQUE NOT NULL,
    Password VARCHAR(100) NOT NULL,
    CreateDate DATETIME DEFAULT GETDATE()
);

-- Sample Data
INSERT INTO Users (Name, Role, Email, Password)
VALUES
('Admin User', 'Admin', 'admin@gmail.com', 'admin123'),
('Resolver User', 'Resolver', 'resolver@gmail.com', 'resolver123'),
('Reviewer User', 'Reviewer', 'reviewer@gmail.com', 'reviewer123');

-----------------------------------------------------------

-- 2️⃣ CATEGORY TABLE
CREATE TABLE Category (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName VARCHAR(100) NOT NULL
);

-----------------------------------------------------------

-- 3️⃣ PRIORITY TABLE
CREATE TABLE Priority (
    PriorityID INT IDENTITY(1,1) PRIMARY KEY,
    PriorityLevel VARCHAR(50) NOT NULL
);

-----------------------------------------------------------

-- 4️⃣ TICKETS TABLE
CREATE TABLE Tickets (
    TicketID INT IDENTITY(1,1) PRIMARY KEY,
    Title VARCHAR(200) NOT NULL,
    Description VARCHAR(1000),
    CategoryID INT FOREIGN KEY REFERENCES Category(CategoryID),
    PriorityID INT FOREIGN KEY REFERENCES Priority(PriorityID),
    Status VARCHAR(50) DEFAULT 'Open',
    CreatedBy INT FOREIGN KEY REFERENCES Users(UserID),
    CreatedDate DATETIME DEFAULT GETDATE(),
    ClosedDate DATETIME NULL,
    UpdatedDate DATETIME NULL,
    AdminRole VARCHAR(50)
);

-----------------------------------------------------------

-- 5️⃣ TICKET ALLOCATION TABLE
CREATE TABLE TicketAllocation (
    AllocationID INT IDENTITY(1,1) PRIMARY KEY,
    TicketID INT NOT NULL FOREIGN KEY REFERENCES Tickets(TicketID),
    UserID INT NOT NULL FOREIGN KEY REFERENCES Users(UserID),
    AllocationRole VARCHAR(50),
    AllocationDate DATETIME DEFAULT GETDATE(),
    Status VARCHAR(50) DEFAULT 'Assigned'
);

-----------------------------------------------------------

-- 6️⃣ AUDIT LOG TABLE
CREATE TABLE AuditLog (
    LogID INT IDENTITY(1,1) PRIMARY KEY,
    TicketID INT NOT NULL FOREIGN KEY REFERENCES Tickets(TicketID),
    ActionTaken VARCHAR(200),
    ActionBy INT FOREIGN KEY REFERENCES Users(UserID),
    ActionDate DATETIME DEFAULT GETDATE()
);

-----------------------------------------------------------

-- 7️⃣ NOTIFICATION TABLE
CREATE TABLE Notification (
    NotificationID INT IDENTITY(1,1) PRIMARY KEY,
    TicketID INT NOT NULL FOREIGN KEY REFERENCES Tickets(TicketID),
    UserID INT NOT NULL FOREIGN KEY REFERENCES Users(UserID),
    Message VARCHAR(200) NOT NULL,
    Status VARCHAR(20) DEFAULT 'Sent'
        CHECK (Status IN ('Sent', 'Read')),
    Timestamp DATETIME DEFAULT GETDATE()
);

select * from Users;
select * from Category;
select * from Priority;
select * from Tickets;
select * from TicketAllocation;
select * from AuditLog;
select * from Notification;