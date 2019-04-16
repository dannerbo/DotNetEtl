CREATE PROCEDURE [dbo].[RaiseError]
AS
BEGIN
	RAISERROR ('Test error', 16, 1)
END
