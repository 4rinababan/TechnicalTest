/*
=======================================================
 05_InsertDummyData.sql
 Seed data: User (Admin & Supplier), Supplier, UserMapping

 Skema password (akan dipakai juga di C# Login logic):
   PasswordHash = SHA256( Password + PasswordSalt )  -> HEX UPPERCASE
   PasswordSalt = 'TTS2026SALT'   (contoh salt statis untuk seed;
                                    untuk produksi sebaiknya salt unik per user)

 Akun default:
   Admin     / Admin@123
   supplier1 / Supplier@123   (mapping ke SUP-001)
   supplier2 / Supplier@123   (mapping ke SUP-002)
=======================================================
*/

USE TechnicalTestDB;
GO

------------------------------------------------------
-- USERS
------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM dbo.[User] WHERE Username = 'admin')
BEGIN
    INSERT INTO dbo.[User] (Username, PasswordHash, PasswordSalt, FullName, Role, IsActive)
    VALUES ('admin', 'D1C13820FE26D402C7D141E8E35734E457B1C965B2E263E4A7EEFA0AD16F2F21', 'TTS2026SALT', 'Administrator', 'Admin', 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.[User] WHERE Username = 'supplier1')
BEGIN
    INSERT INTO dbo.[User] (Username, PasswordHash, PasswordSalt, FullName, Role, IsActive)
    VALUES ('supplier1', 'C36EE6101C64F5AAAED13AF6D89AF9DF98CA9F6858B21FB86801D8A2E02CEB79', 'TTS2026SALT', 'Supplier User Satu', 'Supplier', 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.[User] WHERE Username = 'supplier2')
BEGIN
    INSERT INTO dbo.[User] (Username, PasswordHash, PasswordSalt, FullName, Role, IsActive)
    VALUES ('supplier2', 'C36EE6101C64F5AAAED13AF6D89AF9DF98CA9F6858B21FB86801D8A2E02CEB79', 'TTS2026SALT', 'Supplier User Dua', 'Supplier', 1);
END
GO

------------------------------------------------------
-- SUPPLIERS
------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM dbo.Supplier WHERE SupplierCode = 'SUP-001')
BEGIN
    INSERT INTO dbo.Supplier (SupplierCode, SupplierName, Address, City, Phone, Email, IsActive, CreatedBy)
    VALUES ('SUP-001', 'PT Sumber Makmur', 'Jl. Sudirman No. 1', 'Jakarta', '021-1234567', 'contact@sumbermakmur.co.id', 1,
            (SELECT UserID FROM dbo.[User] WHERE Username = 'admin'));
END

IF NOT EXISTS (SELECT 1 FROM dbo.Supplier WHERE SupplierCode = 'SUP-002')
BEGIN
    INSERT INTO dbo.Supplier (SupplierCode, SupplierName, Address, City, Phone, Email, IsActive, CreatedBy)
    VALUES ('SUP-002', 'CV Karya Abadi', 'Jl. Diponegoro No. 5', 'Bandung', '022-7654321', 'info@karyaabadi.co.id', 1,
            (SELECT UserID FROM dbo.[User] WHERE Username = 'admin'));
END

IF NOT EXISTS (SELECT 1 FROM dbo.Supplier WHERE SupplierCode = 'SUP-003')
BEGIN
    INSERT INTO dbo.Supplier (SupplierCode, SupplierName, Address, City, Phone, Email, IsActive, CreatedBy)
    VALUES ('SUP-003', 'PT Cahaya Elektronik', 'Jl. Gatot Subroto No. 9', 'Surabaya', '031-9988776', 'sales@cahayaelektronik.co.id', 1,
            (SELECT UserID FROM dbo.[User] WHERE Username = 'admin'));
END

IF NOT EXISTS (SELECT 1 FROM dbo.Supplier WHERE SupplierCode = 'SUP-004')
BEGIN
    -- Sengaja dibuat tanpa mapping user, untuk soal SQL "supplier tanpa mapping"
    INSERT INTO dbo.Supplier (SupplierCode, SupplierName, Address, City, Phone, Email, IsActive, CreatedBy)
    VALUES ('SUP-004', 'UD Berkah Jaya', 'Jl. Ahmad Yani No. 12', 'Jakarta', '021-5566778', 'berkah.jaya@mail.com', 0,
            (SELECT UserID FROM dbo.[User] WHERE Username = 'admin'));
END
GO

------------------------------------------------------
-- USER MAPPING
-- supplier1 -> SUP-001 ; supplier2 -> SUP-002
------------------------------------------------------
IF NOT EXISTS (
    SELECT 1 FROM dbo.UserMapping um
    INNER JOIN dbo.[User] u ON u.UserID = um.UserID
    INNER JOIN dbo.Supplier s ON s.SupplierID = um.SupplierID
    WHERE u.Username = 'supplier1' AND s.SupplierCode = 'SUP-001'
)
BEGIN
    INSERT INTO dbo.UserMapping (UserID, SupplierID)
    VALUES (
        (SELECT UserID FROM dbo.[User] WHERE Username = 'supplier1'),
        (SELECT SupplierID FROM dbo.Supplier WHERE SupplierCode = 'SUP-001')
    );
END

IF NOT EXISTS (
    SELECT 1 FROM dbo.UserMapping um
    INNER JOIN dbo.[User] u ON u.UserID = um.UserID
    INNER JOIN dbo.Supplier s ON s.SupplierID = um.SupplierID
    WHERE u.Username = 'supplier2' AND s.SupplierCode = 'SUP-002'
)
BEGIN
    INSERT INTO dbo.UserMapping (UserID, SupplierID)
    VALUES (
        (SELECT UserID FROM dbo.[User] WHERE Username = 'supplier2'),
        (SELECT SupplierID FROM dbo.Supplier WHERE SupplierCode = 'SUP-002')
    );
END
GO
