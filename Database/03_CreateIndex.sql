/*
=======================================================
 03_CreateIndex.sql
 Index tambahan untuk optimasi query
 (UNIQUE constraint di tabel sudah otomatis membuat index,
  di sini kita tambahkan index non-clustered untuk kolom
  yang sering dipakai untuk filter/search/join)
=======================================================
*/

USE TechnicalTestDB;
GO

------------------------------------------------------
-- Index untuk pencarian & filter Supplier
------------------------------------------------------
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Supplier_City' AND object_id = OBJECT_ID('dbo.Supplier'))
    DROP INDEX IX_Supplier_City ON dbo.Supplier;
GO
CREATE NONCLUSTERED INDEX IX_Supplier_City ON dbo.Supplier(City);
GO

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Supplier_IsActive' AND object_id = OBJECT_ID('dbo.Supplier'))
    DROP INDEX IX_Supplier_IsActive ON dbo.Supplier;
GO
CREATE NONCLUSTERED INDEX IX_Supplier_IsActive ON dbo.Supplier(IsActive);
GO

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Supplier_Name' AND object_id = OBJECT_ID('dbo.Supplier'))
    DROP INDEX IX_Supplier_Name ON dbo.Supplier;
GO
CREATE NONCLUSTERED INDEX IX_Supplier_Name ON dbo.Supplier(SupplierName);
GO

------------------------------------------------------
-- Index untuk UserMapping (dipakai join RBAC)
------------------------------------------------------
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_UserMapping_UserID' AND object_id = OBJECT_ID('dbo.UserMapping'))
    DROP INDEX IX_UserMapping_UserID ON dbo.UserMapping;
GO
CREATE NONCLUSTERED INDEX IX_UserMapping_UserID ON dbo.UserMapping(UserID);
GO

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_UserMapping_SupplierID' AND object_id = OBJECT_ID('dbo.UserMapping'))
    DROP INDEX IX_UserMapping_SupplierID ON dbo.UserMapping;
GO
CREATE NONCLUSTERED INDEX IX_UserMapping_SupplierID ON dbo.UserMapping(SupplierID);
GO
