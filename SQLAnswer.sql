/*
=======================================================
 SQLAnswer.sql
 Jawaban soal SQL - Technical Test ASP.NET MVC Developer
=======================================================
*/

USE TechnicalTestDB;
GO

------------------------------------------------------
-- 1. Supplier Aktif
------------------------------------------------------
SELECT
    SupplierID,
    SupplierCode,
    SupplierName,
    City,
    IsActive
FROM dbo.Supplier
WHERE IsActive = 1
ORDER BY SupplierName;
GO

------------------------------------------------------
-- 2. Jumlah Supplier per Kota
------------------------------------------------------
SELECT
    ISNULL(City, '(Tidak diketahui)') AS City,
    COUNT(*) AS JumlahSupplier
FROM dbo.Supplier
GROUP BY City
ORDER BY JumlahSupplier DESC;
GO

------------------------------------------------------
-- 3. Supplier Tanpa Mapping (tidak punya UserMapping)
------------------------------------------------------
SELECT
    s.SupplierID,
    s.SupplierCode,
    s.SupplierName,
    s.City,
    s.IsActive
FROM dbo.Supplier s
WHERE NOT EXISTS (
    SELECT 1
    FROM dbo.UserMapping um
    WHERE um.SupplierID = s.SupplierID
)
ORDER BY s.SupplierName;
GO

------------------------------------------------------
-- 4. Duplicate Code
-- Catatan: SupplierCode punya UNIQUE CONSTRAINT (UQ_Supplier_Code) di
-- tabel Supplier, jadi seharusnya TIDAK PERNAH ada duplikat pada kondisi
-- normal. Query ini tetap disediakan sebagai:
--  (a) safety-net/audit query bila suatu saat constraint dilepas,
--      atau saat proses migrasi/import data dari sumber luar yang
--      belum tervalidasi.
--  (b) dasar logika untuk sp_Supplier_CheckDuplicateCode yang dipakai
--      di aplikasi sebelum Insert/Update (lihat 04_CreateStoredProcedure.sql).
------------------------------------------------------
SELECT
    SupplierCode,
    COUNT(*) AS Jumlah
FROM dbo.Supplier
GROUP BY SupplierCode
HAVING COUNT(*) > 1;
GO
