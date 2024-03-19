SET IDENTITY_INSERT Role ON;
INSERT INTO Role(Id,Name)
VALUES (0,'User'),(1,'Admin');
INSERT INTO [User] (Id, ActiveKey, CreateAt, Email, [Password], PhoneNumber, RoleId, UesrName, UrlPhoto,IsActive)
VALUES ('a-d-m-i-n', 'Admin', GETDATE(), 'Admin@Sociala.com', '49f146fc737baa7c77c48463491f35720b32a2513eb6b927f0dd6c86535a43c8', '123456789',1, 'Admin', '/imj/default.jpg',1);
SET IDENTITY_INSERT Role OFF;