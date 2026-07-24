/*
=======================================================
 06_CreateErrorLogTable.sql
 Tabel tambahan (nilai tambah) untuk logging error ke database.
 Bukan bagian dari 3 tabel wajib (Supplier, User, UserMapping).
=======================================================
*/

USE TechnicalTestDB;
GO

IF OBJECT_ID('dbo.ErrorLog', 'U') IS NOT NULL
    DROP TABLE dbo.ErrorLog;
GO

CREATE TABLE dbo.ErrorLog
(
    LogID             INT IDENTITY(1,1)  NOT NULL,
    ControllerName    VARCHAR(100)       NULL,
    ActionName        VARCHAR(100)       NULL,
    ExceptionMessage  NVARCHAR(1000)     NOT NULL,
    StackTrace        NVARCHAR(MAX)      NULL,
    InnerException    NVARCHAR(1000)     NULL,
    Username          VARCHAR(50)        NULL,
    RequestUrl        NVARCHAR(500)      NULL,
    CreatedDate       DATETIME           NOT NULL CONSTRAINT DF_ErrorLog_CreatedDate DEFAULT (GETDATE()),

    CONSTRAINT PK_ErrorLog PRIMARY KEY CLUSTERED (LogID)
);
GO

CREATE NONCLUSTERED INDEX IX_ErrorLog_CreatedDate ON dbo.ErrorLog(CreatedDate DESC);
GO
