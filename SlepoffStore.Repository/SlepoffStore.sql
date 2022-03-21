--
-- File generated with SQLiteStudio v3.3.3 on Пн мар 21 10:47:25 2022
--
-- Text encoding used: UTF-8
--
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: Categories
DROP TABLE IF EXISTS Categories;

CREATE TABLE Categories (
    Id        INTEGER PRIMARY KEY ASC,
    SectionId INTEGER REFERENCES Sections (Id),
    Name      STRING  NOT NULL,
    UNIQUE (
        SectionId,
        Name
    )
);


-- Table: Devices
DROP TABLE IF EXISTS Devices;

CREATE TABLE Devices (
    Id     INTEGER PRIMARY KEY ASC,
    UserId INTEGER REFERENCES Users (Id) 
                   NOT NULL,
    Name   STRING  NOT NULL,
    UNIQUE (
        UserId,
        Name
    )
);


-- Table: Entries
DROP TABLE IF EXISTS Entries;

CREATE TABLE Entries (
    Id           INTEGER  PRIMARY KEY ASC,
    CategoryId   INTEGER  REFERENCES Categories (Id),
    IsDeleted    BOOLEAN  NOT NULL,
    CreationDate DATETIME NOT NULL,
    Color        STRING   NOT NULL,
    Caption      STRING   NOT NULL,
    Text         TEXT,
    Alarm        DATETIME,
    AlarmIsOn    BOOLEAN  NOT NULL
                          DEFAULT (FALSE) 
);


-- Table: KeyValues
DROP TABLE IF EXISTS KeyValues;

CREATE TABLE KeyValues (
    Id     INTEGER PRIMARY KEY ASC,
    UserId INTEGER REFERENCES Users (Id) 
                   NOT NULL,
    [Key]  STRING  NOT NULL,
    Value  STRING,
    UNIQUE (
        UserId,
        [Key]
    )
);


-- Table: Sections
DROP TABLE IF EXISTS Sections;

CREATE TABLE Sections (
    Id     INTEGER PRIMARY KEY ASC,
    UserId INTEGER REFERENCES Users (Id) 
                   NOT NULL,
    Name   STRING  UNIQUE
                   NOT NULL,
    UNIQUE (
        UserId,
        Name
    )
);


-- Table: UISheets
DROP TABLE IF EXISTS UISheets;

CREATE TABLE UISheets (
    Id       INTEGER PRIMARY KEY ASC,
    EntryId  INTEGER REFERENCES Entries (Id) 
                     NOT NULL,
    DeviceId INTEGER REFERENCES Devices (Id) 
                     NOT NULL,
    PosX     INTEGER NOT NULL,
    PosY     INTEGER NOT NULL,
    Width    INTEGER NOT NULL,
    Height   INTEGER NOT NULL,
    UNIQUE (
        EntryId,
        DeviceId
    )
);


-- Table: Users
DROP TABLE IF EXISTS Users;

CREATE TABLE Users (
    Id       INTEGER PRIMARY KEY ASC,
    Name     STRING  UNIQUE
                     NOT NULL,
    Password STRING  NOT NULL,
    Comments STRING
);


COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
