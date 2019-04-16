CREATE PROCEDURE [dbo].[InsertPeople]
	@People dbo.PersonType READONLY
AS
BEGIN
	INSERT dbo.Person (FirstName, LastName, MiddleInitial, Age, DateOfBirth, Gender)
	SELECT p.FirstName, p.LastName, p.MiddleInitial, p.Age, p.DateOfBirth, p.Gender
	FROM @People p
END
