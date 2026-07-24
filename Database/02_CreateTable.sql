/*
=======================================================
 02_CreateTable.sql
 Membuat tabel: [User], Supplier, UserMapping
 Beserta Primary Key, Foreign Key, dan Constraint
=======================================================
*/

USE TechnicalTestDB;
GO

------------------------------------------------------
-- TABLE: [User]
------------------------------------------------------
IF OBJECT_ID('dbo.[User]', 'U') IS NOT NULL
    DROP TABLE dbo.[User];
GO

CREATE TABLE dbo.[User]
(
    UserID          INT IDENTITY(1,1)   NOT NULL,
    Username        VARCHAR(50)         NOT NULL,
    PasswordHash    VARCHAR(256)        NOT NULL,
    PasswordSalt    VARCHAR(256)        NOT NULL,
    FullName        VARCHAR(100)        NOT NULL,
    Role            VARCHAR(20)         NOT NULL,
    IsActive        BIT                 NOT NULL CONSTRAINT DF_User_IsActive DEFAULT (1),
    CreatedDate     DATETIME            NOT NULL CONSTRAINT DF_User_CreatedDate DEFAULT (GETDATE()),
    ModifiedDate    DATETIME            NULL,

    CONSTRAINT PK_User PRIMARY KEY CLUSTERED (UserID),
    CONSTRAINT UQ_User_Username UNIQUE (Username),
    CONSTRAINT CK_User_Role CHECK (Role IN ('Admin', 'Supplier'))
);
GO

------------------------------------------------------
-- TABLE: Supplier
------------------------------------------------------
IF OBJECT_ID('dbo.Supplier', 'U') IS NOT NULL
    DROP TABLE dbo.Supplier;
GO

CREATE TABLE dbo.Supplier
(
    SupplierID      INT IDENTITY(1,1)   NOT NULL,
    SupplierCode    VARCHAR(20)         NOT NULL,
    SupplierName    VARCHAR(150)        NOT NULL,
    Address         VARCHAR(255)        NULL,
    City            VARCHAR(100)        NULL,
    Phone           VARCHAR(30)         NULL,
    Email           VARCHAR(100)        NULL,
    IsActive        BIT                 NOT NULL CONSTRAINT DF_Supplier_IsActive DEFAULT (1),
    CreatedBy       INT                 NULL,
    CreatedDate     DATETIME            NOT NULL CONSTRAINT DF_Supplier_CreatedDate DEFAULT (GETDATE()),
    ModifiedBy      INT                 NULL,
    ModifiedDate    DATETIME            NULL,

    CONSTRAINT PK_Supplier PRIMARY KEY CLUSTERED (SupplierID),
    CONSTRAINT UQ_Supplier_Code UNIQUE (SupplierCode),
    CONSTRAINT FK_Supplier_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.[User](UserID),
    CONSTRAINT FK_Supplier_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES dbo.[User](UserID)
);
GO

------------------------------------------------------
-- TABLE: UserMapping
-- (menghubungkan User dengan Role = 'Supplier' ke data Supplier miliknya)
------------------------------------------------------
IF OBJECT_ID('dbo.UserMapping', 'U') IS NOT NULL
    DROP TABLE dbo.UserMapping;
GO

CREATE TABLE dbo.UserMapping
(
    UserMappingID   INT IDENTITY(1,1)   NOT NULL,
    UserID          INT                 NOT NULL,
    SupplierID      INT                 NOT NULL,
    CreatedDate     DATETIME            NOT NULL CONSTRAINT DF_UserMapping_CreatedDate DEFAULT (GETDATE()),

    CONSTRAINT PK_UserMapping PRIMARY KEY CLUSTERED (UserMappingID),
    CONSTRAINT FK_UserMapping_User FOREIGN KEY (UserID) REFERENCES dbo.[User](UserID) ON DELETE CASCADE,
    CONSTRAINT FK_UserMapping_Supplier FOREIGN KEY (SupplierID) REFERENCES dbo.Supplier(SupplierID) ON DELETE CASCADE,
    CONSTRAINT UQ_UserMapping_User_Supplier UNIQUE (UserID, SupplierID)
);
GO
