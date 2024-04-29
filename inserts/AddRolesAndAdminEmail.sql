SET IDENTITY_INSERT Role ON;
INSERT INTO Role(Id,Name)
VALUES (1,'User'),(2,'Admin');
INSERT INTO [User] (Id, ActiveKey, CreateAt, Email, [Password], PhoneNumber, RoleId, UesrName, UrlPhoto,IsActive)
VALUES ('a-d-m-i-n', 'Admin', GETDATE(), 'Admin@Sociala.com', '3ad0dc71aff35754d26de04a4057b702a955a50b94d1c6586f8d353928186974', '123456789',2, 'Admin', '/imj/default.jpg',1);
SET IDENTITY_INSERT Role OFF;
--Admin unhashed password: MMaasm4@
--users unhashed password: Hamo1212#