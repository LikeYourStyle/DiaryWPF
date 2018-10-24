CREATE PROCEDURE [dbo.sp_InsertUser]
	@login varchar(50),
	@password varchar(50),
	@id int out
AS
INSERT INTO Users (login, password) VALUES (@login, @password)

SET @id = SCOPE_IDENTITY()
GO