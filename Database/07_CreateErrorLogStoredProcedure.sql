/*
=======================================================
 07_CreateErrorLogStoredProcedure.sql
 Stored procedure tambahan untuk menyimpan error log.
=======================================================
*/

USE TechnicalTestDB;
GO

IF OBJECT_ID('dbo.sp_ErrorLog_Insert', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_ErrorLog_Insert;
GO
CREATE PROCEDURE dbo.sp_ErrorLog_Insert
    @ControllerName   VARCHAR(100)   = NULL,
    @ActionName       VARCHAR(100)   = NULL,
    @ExceptionMessage NVARCHAR(1000),
    @StackTrace       NVARCHAR(MAX)  = NULL,
    @InnerException   NVARCHAR(1000) = NULL,
    @Username         VARCHAR(50)    = NULL,
    @RequestUrl       NVARCHAR(500)  = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.ErrorLog
        (ControllerName, ActionName, ExceptionMessage, StackTrace, InnerException, Username, RequestUrl, CreatedDate)
    VALUES
        (@ControllerName, @ActionName, @ExceptionMessage, @StackTrace, @InnerException, @Username, @RequestUrl, GETDATE());
END
GO

------------------------------------------------------
-- sp_ErrorLog_GetList
-- Untuk halaman viewer log (opsional, memudahkan review saat testing)
------------------------------------------------------
IF OBJECT_ID('dbo.sp_ErrorLog_GetList', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_ErrorLog_GetList;
GO
CREATE PROCEDURE dbo.sp_ErrorLog_GetList
    @Top INT = 100
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (@Top)
        LogID, ControllerName, ActionName, ExceptionMessage, Username, RequestUrl, CreatedDate
    FROM dbo.ErrorLog
    ORDER BY CreatedDate DESC;
END
GO
