/*
=======================================================
 04_CreateStoredProcedure.sql
 Seluruh Stored Procedure wajib:
 - sp_Login
 - sp_Supplier_GetList
 - sp_Supplier_GetById
 - sp_Supplier_Insert
 - sp_Supplier_Update
 - sp_Supplier_Delete
 - sp_Supplier_Search
 - sp_UserMapping_GetSupplier
 - sp_Supplier_CheckDuplicateCode

 Catatan RBAC:
 - Role 'Admin'    -> bisa akses semua data Supplier
 - Role 'Supplier' -> hanya bisa akses Supplier yang ada di UserMapping miliknya
=======================================================
*/

USE TechnicalTestDB;
GO

------------------------------------------------------
-- sp_Login
-- SP hanya mengambil kredensial tersimpan berdasarkan Username.
-- Verifikasi hash password (SHA256 + salt) dilakukan di layer
-- aplikasi (C#) karena salt unik per-user, sehingga aplikasi
-- perlu tahu salt lebih dulu sebelum bisa membandingkan hash.
------------------------------------------------------
IF OBJECT_ID('dbo.sp_Login', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_Login;
GO
CREATE PROCEDURE dbo.sp_Login
    @Username VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        u.UserID,
        u.Username,
        u.FullName,
        u.Role,
        u.PasswordHash,
        u.PasswordSalt
    FROM dbo.[User] u
    WHERE u.Username = @Username
      AND u.IsActive = 1;
END
GO

------------------------------------------------------
-- sp_Supplier_GetList
-- List dengan paging, sorting, filter, dan RBAC
------------------------------------------------------
IF OBJECT_ID('dbo.sp_Supplier_GetList', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_Supplier_GetList;
GO
CREATE PROCEDURE dbo.sp_Supplier_GetList
    @UserID        INT,
    @Role          VARCHAR(20),
    @City          VARCHAR(100)  = NULL,
    @IsActive      BIT           = NULL,
    @SortColumn    VARCHAR(50)   = 'SupplierName',
    @SortDirection VARCHAR(4)    = 'ASC',
    @PageNumber    INT           = 1,
    @PageSize      INT           = 10
AS
BEGIN
    SET NOCOUNT ON;

    IF @SortColumn NOT IN ('SupplierCode', 'SupplierName', 'City', 'IsActive', 'CreatedDate')
        SET @SortColumn = 'SupplierName';

    IF @SortDirection NOT IN ('ASC', 'DESC')
        SET @SortDirection = 'ASC';

    ;WITH FilteredSupplier AS
    (
        SELECT s.*
        FROM dbo.Supplier s
        WHERE (@City IS NULL OR s.City = @City)
          AND (@IsActive IS NULL OR s.IsActive = @IsActive)
          AND (
                @Role = 'Admin'
                OR EXISTS (
                    SELECT 1 FROM dbo.UserMapping um
                    WHERE um.SupplierID = s.SupplierID
                      AND um.UserID = @UserID
                )
              )
    )
    SELECT
        SupplierID, SupplierCode, SupplierName, Address, City, Phone, Email,
        IsActive, CreatedDate, ModifiedDate,
        COUNT(*) OVER() AS TotalRecords
    FROM FilteredSupplier
    ORDER BY
        CASE WHEN @SortColumn = 'SupplierCode' AND @SortDirection = 'ASC'  THEN SupplierCode END ASC,
        CASE WHEN @SortColumn = 'SupplierCode' AND @SortDirection = 'DESC' THEN SupplierCode END DESC,
        CASE WHEN @SortColumn = 'SupplierName' AND @SortDirection = 'ASC'  THEN SupplierName END ASC,
        CASE WHEN @SortColumn = 'SupplierName' AND @SortDirection = 'DESC' THEN SupplierName END DESC,
        CASE WHEN @SortColumn = 'City' AND @SortDirection = 'ASC'  THEN City END ASC,
        CASE WHEN @SortColumn = 'City' AND @SortDirection = 'DESC' THEN City END DESC,
        CASE WHEN @SortColumn = 'IsActive' AND @SortDirection = 'ASC'  THEN IsActive END ASC,
        CASE WHEN @SortColumn = 'IsActive' AND @SortDirection = 'DESC' THEN IsActive END DESC,
        CASE WHEN @SortColumn = 'CreatedDate' AND @SortDirection = 'ASC'  THEN CreatedDate END ASC,
        CASE WHEN @SortColumn = 'CreatedDate' AND @SortDirection = 'DESC' THEN CreatedDate END DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY
END
GO

------------------------------------------------------
-- sp_Supplier_GetById
-- Ambil 1 supplier, dengan pengecekan RBAC
------------------------------------------------------
IF OBJECT_ID('dbo.sp_Supplier_GetById', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_Supplier_GetById;
GO
CREATE PROCEDURE dbo.sp_Supplier_GetById
    @SupplierID INT,
    @UserID     INT,
    @Role       VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT s.SupplierID, s.SupplierCode, s.SupplierName, s.Address, s.City,
           s.Phone, s.Email, s.IsActive, s.CreatedDate, s.ModifiedDate
    FROM dbo.Supplier s
    WHERE s.SupplierID = @SupplierID
      AND (
            @Role = 'Admin'
            OR EXISTS (
                SELECT 1 FROM dbo.UserMapping um
                WHERE um.SupplierID = s.SupplierID
                  AND um.UserID = @UserID
            )
          );
END
GO

------------------------------------------------------
-- sp_Supplier_Insert
------------------------------------------------------
IF OBJECT_ID('dbo.sp_Supplier_Insert', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_Supplier_Insert;
GO
CREATE PROCEDURE dbo.sp_Supplier_Insert
    @SupplierCode VARCHAR(20),
    @SupplierName VARCHAR(150),
    @Address      VARCHAR(255) = NULL,
    @City         VARCHAR(100) = NULL,
    @Phone        VARCHAR(30)  = NULL,
    @Email        VARCHAR(100) = NULL,
    @CreatedBy    INT,
    @NewSupplierID INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Supplier
        (SupplierCode, SupplierName, Address, City, Phone, Email, IsActive, CreatedBy, CreatedDate)
    VALUES
        (@SupplierCode, @SupplierName, @Address, @City, @Phone, @Email, 1, @CreatedBy, GETDATE());

    SET @NewSupplierID = SCOPE_IDENTITY();
END
GO

------------------------------------------------------
-- sp_Supplier_Update
------------------------------------------------------
IF OBJECT_ID('dbo.sp_Supplier_Update', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_Supplier_Update;
GO
CREATE PROCEDURE dbo.sp_Supplier_Update
    @SupplierID   INT,
    @SupplierCode VARCHAR(20),
    @SupplierName VARCHAR(150),
    @Address      VARCHAR(255) = NULL,
    @City         VARCHAR(100) = NULL,
    @Phone        VARCHAR(30)  = NULL,
    @Email        VARCHAR(100) = NULL,
    @IsActive     BIT,
    @ModifiedBy   INT,
    @UserID       INT,
    @Role         VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE s
    SET SupplierCode = @SupplierCode,
        SupplierName = @SupplierName,
        Address      = @Address,
        City         = @City,
        Phone        = @Phone,
        Email        = @Email,
        IsActive     = @IsActive,
        ModifiedBy   = @ModifiedBy,
        ModifiedDate = GETDATE()
    FROM dbo.Supplier s
    WHERE s.SupplierID = @SupplierID
      AND (
            @Role = 'Admin'
            OR EXISTS (
                SELECT 1 FROM dbo.UserMapping um
                WHERE um.SupplierID = s.SupplierID
                  AND um.UserID = @UserID
            )
          );

    SELECT @@ROWCOUNT AS AffectedRows;
END
GO

------------------------------------------------------
-- sp_Supplier_Delete
-- Soft delete (IsActive = 0), hanya Admin yang diizinkan
------------------------------------------------------
IF OBJECT_ID('dbo.sp_Supplier_Delete', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_Supplier_Delete;
GO
CREATE PROCEDURE dbo.sp_Supplier_Delete
    @SupplierID INT,
    @ModifiedBy INT,
    @Role       VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Role <> 'Admin'
    BEGIN
        SELECT 0 AS AffectedRows;
        RETURN;
    END

    UPDATE dbo.Supplier
    SET IsActive = 0,
        ModifiedBy = @ModifiedBy,
        ModifiedDate = GETDATE()
    WHERE SupplierID = @SupplierID;

    SELECT @@ROWCOUNT AS AffectedRows;
END
GO

------------------------------------------------------
-- sp_Supplier_Search
-- Free text search (code / name), dengan RBAC + paging
------------------------------------------------------
IF OBJECT_ID('dbo.sp_Supplier_Search', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_Supplier_Search;
GO
CREATE PROCEDURE dbo.sp_Supplier_Search
    @Keyword    VARCHAR(150),
    @UserID     INT,
    @Role       VARCHAR(20),
    @PageNumber INT = 1,
    @PageSize   INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH FilteredSupplier AS
    (
        SELECT s.*
        FROM dbo.Supplier s
        WHERE (s.SupplierCode LIKE '%' + @Keyword + '%'
               OR s.SupplierName LIKE '%' + @Keyword + '%')
          AND (
                @Role = 'Admin'
                OR EXISTS (
                    SELECT 1 FROM dbo.UserMapping um
                    WHERE um.SupplierID = s.SupplierID
                      AND um.UserID = @UserID
                )
              )
    )
    SELECT
        SupplierID, SupplierCode, SupplierName, Address, City, Phone, Email,
        IsActive, CreatedDate, ModifiedDate,
        COUNT(*) OVER() AS TotalRecords
    FROM FilteredSupplier
    ORDER BY SupplierName ASC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO

------------------------------------------------------
-- sp_UserMapping_GetSupplier
-- Ambil daftar SupplierID milik seorang User (role Supplier)
------------------------------------------------------
IF OBJECT_ID('dbo.sp_UserMapping_GetSupplier', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_UserMapping_GetSupplier;
GO
CREATE PROCEDURE dbo.sp_UserMapping_GetSupplier
    @UserID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT um.UserMappingID, um.UserID, um.SupplierID, s.SupplierCode, s.SupplierName
    FROM dbo.UserMapping um
    INNER JOIN dbo.Supplier s ON s.SupplierID = um.SupplierID
    WHERE um.UserID = @UserID;
END
GO

------------------------------------------------------
-- sp_Supplier_CheckDuplicateCode
-- Dipakai sebelum Insert/Update untuk validasi kode unik
------------------------------------------------------
IF OBJECT_ID('dbo.sp_Supplier_CheckDuplicateCode', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_Supplier_CheckDuplicateCode;
GO
CREATE PROCEDURE dbo.sp_Supplier_CheckDuplicateCode
    @SupplierCode VARCHAR(20),
    @SupplierID   INT = NULL   -- NULL saat Insert, diisi saat Update (exclude diri sendiri)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CASE WHEN EXISTS (
        SELECT 1 FROM dbo.Supplier
        WHERE SupplierCode = @SupplierCode
          AND (@SupplierID IS NULL OR SupplierID <> @SupplierID)
    ) THEN 1 ELSE 0 END AS IsDuplicate
END
GO
