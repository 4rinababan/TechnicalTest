/*
=======================================================
 01_CreateDatabase.sql
 Technical Test - ASP.NET MVC Developer
 Membuat database TechnicalTestDB
=======================================================
*/

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'TechnicalTestDB')
BEGIN
    CREATE DATABASE TechnicalTestDB;
END
GO

ALTER DATABASE TechnicalTestDB SET RECOVERY SIMPLE;
GO

USE TechnicalTestDB;
GO
