/*
drop table ETradeProductOrders
drop table ETradeProducts
drop table ETradeOrders
drop table ETradeCategories
drop table ETradeUsers
drop table ETradeRoles
drop table ETradeUserDetails
drop table ETradeCities
drop table ETradeCountries
truncate table __EFMigrationsHistory
*/

delete from ETradeProductOrders
delete from ETradeProducts
delete from ETradeOrders
delete from ETradeCategories
delete from ETradeUsers
delete from ETradeRoles
delete from ETradeUserDetails
delete from ETradeCities
delete from ETradeCountries

--dbcc CHECKIDENT ('ETradeProductOrders', RESEED, 0)
--dbcc CHECKIDENT ('ETradeProducts', RESEED, 0)
--dbcc CHECKIDENT ('ETradeOrders', RESEED, 0)
--dbcc CHECKIDENT ('ETradeCategories', RESEED, 0)
--dbcc CHECKIDENT ('ETradeUsers', RESEED, 0)
--dbcc CHECKIDENT ('ETradeRoles', RESEED, 0)
--dbcc CHECKIDENT ('ETradeUserDetails', RESEED, 0)
--dbcc CHECKIDENT ('ETradeCities', RESEED, 0)
--dbcc CHECKIDENT ('ETradeCountries', RESEED, 0)

SET IDENTITY_INSERT [dbo].[ETradeCategories] ON 
INSERT [dbo].[ETradeCategories] ([Id], [Name]) VALUES (1, N'Snacks')
INSERT [dbo].[ETradeCategories] ([Id], [Name]) VALUES (2, N'Beverages')
SET IDENTITY_INSERT [dbo].[ETradeCategories] OFF
GO
SET IDENTITY_INSERT [dbo].[ETradeProducts] ON 
INSERT [dbo].[ETradeProducts] ([Id], [Name], [UnitPrice], [StockAmount], [CategoryId], [ExpirationDate]) VALUES (1, N'Lays Classic', 6.90, 300, 1, '2022-12-13')
INSERT [dbo].[ETradeProducts] ([Id], [Name], [UnitPrice], [StockAmount], [CategoryId], [ExpirationDate], [Description]) VALUES (2, N'Coke', 5, 200, 2, '2022-11-23', 'Coca-Cola Glass Zero Sugar')
INSERT [dbo].[ETradeProducts] ([Id], [Name], [UnitPrice], [StockAmount], [CategoryId], [Description]) VALUES (3, N'Browni', 3, 500, 1, 'Eti Browni Gold Chocolate')
INSERT [dbo].[ETradeProducts] ([Id], [Name], [UnitPrice], [StockAmount], [CategoryId]) VALUES (4, N'BeylerBeyi Mineral Water', 3.90, 1000, 2)
INSERT [dbo].[ETradeProducts] ([Id], [Name], [UnitPrice], [StockAmount], [CategoryId]) VALUES (5, N'Ruffles Originals', 6.90, 100, 1)
INSERT [dbo].[ETradeProducts] ([Id], [Name], [UnitPrice], [StockAmount], [CategoryId], [Description]) VALUES (10, N'Ülker Chocolate', 2.85, 500, 1, 'Ülker Chocolate Wafer.')
INSERT [dbo].[ETradeProducts] ([Id], [Name], [UnitPrice], [StockAmount], [CategoryId]) VALUES (11, N'Eker Ayran', 1000, 2.75, 2)
SET IDENTITY_INSERT [dbo].[ETradeProducts] OFF
GO
set identity_insert ETradeRoles on
insert into ETradeRoles (Id, Name) values (1, 'Admin')
insert into ETradeRoles (Id, Name) values (2, 'User')
set identity_insert ETradeRoles off
go
set identity_insert ETradeCountries on
insert into ETradeCountries (Id, Name) values (1, 'Turkey')
insert into ETradeCountries (Id, Name) values (2, 'England')
set identity_insert ETradeCountries off
go
set identity_insert ETradeCities on
insert into ETradeCities (Id, Name, CountryId) values (1, 'Ankara', 1)
insert into ETradeCities (Id, Name, CountryId) values (2, 'Istanbul', 1)
insert into ETradeCities (Id, Name, CountryId) values (3, 'Izmir', 1)
insert into ETradeCities (Id, Name, CountryId) values (4, 'London', 2)
set identity_insert ETradeCities off
go
set identity_insert ETradeUserDetails on
insert into ETradeUserDetails (Id, EMail, CountryId, CityId, Address) values (1, 'neseilhan.com', 1, 1, 'Karşıyaka')
insert into ETradeUserDetails (Id, EMail, CountryId, CityId, Address) values (2, 'lisasmith.com', 2, 4, 'Manchester')
set identity_insert ETradeUserDetails off
go
set identity_insert ETradeUsers on
insert into ETradeUsers (Id, UserName, Password, Active, RoleId, UserDetailId) values (1, 'nese', 'nese', 1, 1, 1)
insert into ETradeUsers (Id, UserName, Password, Active, RoleId, UserDetailId) values (2, 'lisa', 'lisa', 1, 2, 2)
set identity_insert ETradeUsers off
go