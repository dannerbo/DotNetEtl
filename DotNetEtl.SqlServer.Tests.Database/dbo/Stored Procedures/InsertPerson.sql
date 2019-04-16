CREATE PROCEDURE [dbo].[InsertPerson]
	@FirstName VARCHAR(50), 
    @LastName VARCHAR(50), 
    @MiddleInitial CHAR(1), 
    @Age TINYINT, 
    @DateOfBirth DATE, 
	@Gender TINYINT 
AS
BEGIN
	INSERT dbo.Person (FirstName, LastName, MiddleInitial, Age, DateOfBirth, Gender)
	VALUES (@FirstName, @LastName, @MiddleInitial, @Age, @DateOfBirth, @Gender)
END

