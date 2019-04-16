CREATE TYPE [dbo].[PersonType] AS TABLE
(
    [FirstName] VARCHAR(50) NOT NULL, 
    [LastName] VARCHAR(50) NOT NULL, 
    [MiddleInitial] CHAR(1) NULL, 
    [Age] TINYINT NOT NULL, 
    [DateOfBirth] DATE NOT NULL, 
	[Gender] TINYINT NOT NULL
);
