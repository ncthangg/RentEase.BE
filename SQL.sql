Create Database RentEase

-- Bảng phụ
CREATE TABLE Utility (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UtilityName NVARCHAR(100) NOT NULL UNIQUE,
    Note NVARCHAR(255) NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NULL,
);
CREATE TABLE AptCategory (
    Id INT PRIMARY KEY IDENTITY(1,1),
    CategoryName NVARCHAR(100) NOT NULL UNIQUE,
    Note NVARCHAR(255) NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NULL,
);
CREATE TABLE AptStatus (
    Id INT PRIMARY KEY IDENTITY(1,1),
    StatusName NVARCHAR(100) NOT NULL UNIQUE,
    Note NVARCHAR(255) NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NULL,
);
CREATE TABLE Role (
    Id INT PRIMARY KEY IDENTITY(1,1),
    RoleName NVARCHAR(100) NOT NULL UNIQUE,
    Note NVARCHAR(255) NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NULL,
);
CREATE TABLE TransactionType (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TypeName NVARCHAR(100) NOT NULL UNIQUE,
    Note NVARCHAR(255) NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NULL,
);

-- Bảng chính
CREATE TABLE Account (
    AccountId NVARCHAR(255) PRIMARY KEY,
    FullName NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) UNIQUE NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    PhoneNumber NVARCHAR(15) UNIQUE NULL,
    DateOfBirth DATE NULL,
    Gender NVARCHAR(10) NULL,
    AvatarUrl NVARCHAR(MAX) NULL,
    RoleId INT NOT NULL,
    IsActive BIT DEFAULT 0,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NULL,
	DeletedAt DATETIME NULL,
	Status Bit DEFAULT 1 
    CONSTRAINT FK_Account_Role FOREIGN KEY (RoleId) REFERENCES Role(Id)
);
CREATE INDEX IX_Account_Email ON Account (Email);
CREATE INDEX IX_Account_PhoneNumber ON Account (PhoneNumber);
CREATE INDEX IX_Account_RoleId ON Account (RoleId);

CREATE TABLE Apt (
    AptId NVARCHAR(255) PRIMARY KEY,
    OwnerId NVARCHAR(255) NOT NULL,
    Name NVARCHAR(255) NOT NULL,
	Area FLOAT DEFAULT 0,
    Address NVARCHAR(500) NOT NULL,
	AddressLink NVARCHAR(500) NULL,
    ProvinceId INT NOT NULL,
    DistrictId INT NOT NULL,
	WardId INT NOT NULL,
    RentPrice BIGINT NOT NULL,
    PilePrice BIGINT NULL,
    AptCategoryId INT NOT NULL,
    AptStatusId INT NOT NULL,
    NumberOfRoom INT NOT NULL,
    NumberOfSlot INT NOT NULL,
    StatusId INT NOT NULL,
    Note NVARCHAR(MAX) NULL,
    Rating FLOAT DEFAULT 0,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NULL,
	DeletedAt DATETIME NULL,
	Status Bit DEFAULT 0 
    CONSTRAINT FK_Apt_AptCategory FOREIGN KEY (AptCategoryId) REFERENCES AptCategory(Id),
    CONSTRAINT FK_Apt_AptStatus FOREIGN KEY (AptStatusId) REFERENCES AptStatus(Id)
);
CREATE INDEX IX_Apt_OwnerId ON Apt (OwnerId);
CREATE INDEX IX_Apt_AptCategory ON Apt (AptCategoryId);
CREATE INDEX IX_Apt_AptStatus ON Apt (AptStatusId);

CREATE TABLE AptImage (
    AptId NVARCHAR(255) PRIMARY KEY,
    ImageURL1 NVARCHAR(max) NULL,
    ImageURL2 NVARCHAR(max) NULL,
    ImageURL3 NVARCHAR(max) NULL,
	ImageURL4 NVARCHAR(max) NULL,
	ImageURL5 NVARCHAR(max) NULL,
	ImageURL6 NVARCHAR(max) NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NULL,
    CONSTRAINT FK_AptImage_Apt FOREIGN KEY (AptId) REFERENCES Apt(AptId)
);

CREATE TABLE AptUtility (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    AptId NVARCHAR(255) NOT NULL,
    UtilityId INT NOT NULL,
	Note NVARCHAR(500) NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NULL,
    CONSTRAINT FK_AptUtility_Apt FOREIGN KEY (AptId) REFERENCES Apt(AptId) ON DELETE CASCADE,
    CONSTRAINT FK_AptUtility_Utility FOREIGN KEY (UtilityId) REFERENCES Utility(Id) ON DELETE CASCADE,
    CONSTRAINT UQ_AptUtility UNIQUE (AptId, UtilityId) 
);
CREATE INDEX IX_AptUtility_AptId ON AptUtility(AptId);
CREATE INDEX IX_AptUtility_UtilityId ON AptUtility(UtilityId);

CREATE TABLE AccountVerification (
    Id INT PRIMARY KEY IDENTITY(1,1),
    AccountId NVARCHAR(255) NOT NULL,
    VerificationCode NVARCHAR(50) NOT NULL,
    IsUsed BIT DEFAULT 0,
    ExpiresAt DATETIME NOT NULL,
    CreatedAt DATETIME NOT NULL,
    CONSTRAINT FK_AccountVerification_Account FOREIGN KEY (AccountId) REFERENCES Account(AccountId)
);

CREATE TABLE AccountToken (
    Id INT PRIMARY KEY IDENTITY(1,1),
    AccountId NVARCHAR(255) NOT NULL,
    RefreshToken NVARCHAR(MAX) NOT NULL,
    ExpiresAt DATETIME NOT NULL,
    CreatedAt DATETIME NOT NULL,
    CONSTRAINT FK_AccountToken_Account FOREIGN KEY (AccountId) REFERENCES Account(AccountId)
);
CREATE INDEX IX_AccountToken_AccountId ON AccountToken (AccountId);

CREATE TABLE Review (
    Id INT PRIMARY KEY IDENTITY(1,1),
    AccountId NVARCHAR(255) NOT NULL,
    AptId NVARCHAR(255) NOT NULL,
    Rating FLOAT DEFAULT 0,
    Comment NVARCHAR(500) NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NULL,
    CONSTRAINT FK_Review_Account FOREIGN KEY (AccountId) REFERENCES Account(AccountId),
    CONSTRAINT FK_Review_Apt FOREIGN KEY (AptId) REFERENCES Apt(AptId)
);
CREATE INDEX IX_Review_AptId ON Review (AptId);
CREATE INDEX IX_Review_AccountId ON Review (AccountId);


CREATE TABLE Orders (
    OrderId NVARCHAR(255) PRIMARY KEY,
    TransactionTypeId INT NOT NULL,
    SenderId NVARCHAR(255) NOT NULL,
	Amount DECIMAL(18,2) NOT NULL,
	IncurredCost DECIMAL(18,2) NOT NULL,
    CreatedAt DATETIME NOT NULL,
	PaidAt DATETIME NULL,
	StatusId INT NOT NULL,
    CONSTRAINT FK_Order_Sender FOREIGN KEY (SenderId) REFERENCES Account(AccountId),
    CONSTRAINT FK_Order_TransactionType FOREIGN KEY (TransactionTypeId) REFERENCES TransactionType(Id) ON DELETE CASCADE,
);

CREATE TABLE dbo.[Transaction] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TransactionTypeId INT NOT NULL,
	OrderId NVARCHAR(255) NOT NULL,
	PaymentAttempt INT NOT NULL,
	PaymentCode NVARCHAR(255),
    TotalAmount DECIMAL(18,2) NOT NULL,
    Note NVARCHAR(500) NULL,
    CreatedAt DATETIME NOT NULL,
	PaidAt DATETIME NULL,
	StatusId INT NOT NULL,
	CONSTRAINT FK_Transaction_Orders FOREIGN KEY (OrderId) REFERENCES Orders(OrderId) ON DELETE CASCADE,
    CONSTRAINT FK_Transaction_TransactionType FOREIGN KEY (TransactionTypeId) REFERENCES TransactionType(Id)
);

CREATE TABLE Post (
    PostId NVARCHAR(255) PRIMARY KEY,
    AccountId NVARCHAR(255) NOT NULL,
    AptId NVARCHAR(255) NOT NULL,
    Title NVARCHAR(255) NOT NULL,
    TotalSlot INT NOT NULL,
	CurrentSlot INT NOT NULL,
	GenderId INT NOT NULL,
	OldId INT NOT NULL,
	Note NVARCHAR(MAX) NULL,   
	MoveInDate DATE NOT NULL,
    MoveOutDate DATE NULL,
    StatusId INT NOT NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NULL,
	DeletedAt DATETIME NULL,
	Status Bit DEFAULT 1 
    CONSTRAINT FK_Post_Apt FOREIGN KEY (AptId) REFERENCES Apt(AptId),
    CONSTRAINT FK_CPost_Account FOREIGN KEY (AccountId) REFERENCES Account(AccountId)
);

CREATE TABLE PostRequire (
    Id INT PRIMARY KEY IDENTITY(1,1),
    AccountId NVARCHAR(255) NOT NULL,
    PostId NVARCHAR(255) NOT NULL,
	Note NVARCHAR(MAX) NULL, 
    StatusId INT NOT NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NULL,
    CONSTRAINT FK_PostRequire_Post FOREIGN KEY (PostId) REFERENCES Post(PostId),
    CONSTRAINT FK_PostRequire_Account FOREIGN KEY (AccountId) REFERENCES Account(AccountId)
);

-- Xóa bảng có quan hệ trước (bảng phụ thuộc)
DROP TABLE IF EXISTS AccountToken;
DROP TABLE IF EXISTS AptImage;
DROP TABLE IF EXISTS Review;
DROP TABLE IF EXISTS Post;
DROP TABLE IF EXISTS PostRequire;
DROP TABLE IF EXISTS dbo.[Orders];
DROP TABLE IF EXISTS Wallet;
DROP TABLE IF EXISTS CurrentResident;
DROP TABLE IF EXISTS MaintenanceRequest;
DROP TABLE IF EXISTS AptUtility;
DROP TABLE IF EXISTS AccountVerification;
DROP TABLE IF EXISTS dbo.[Transaction];
DROP TABLE IF EXISTS dbo.[Contract];
DROP TABLE IF EXISTS Apt;
DROP TABLE IF EXISTS Account;

-- Xóa bảng chính (bảng độc lập)
DROP TABLE IF EXISTS TransactionType;
DROP TABLE IF EXISTS Role;
DROP TABLE IF EXISTS AptStatus;
DROP TABLE IF EXISTS AptCategory;
DROP TABLE IF EXISTS Utility;


DECLARE @sql NVARCHAR(MAX) = '';

SELECT @sql += 'ALTER TABLE ' + QUOTENAME(OBJECT_NAME(parent_object_id)) + 
               ' DROP CONSTRAINT ' + QUOTENAME(name) + ';' + CHAR(13)
FROM sys.foreign_keys;

EXEC sp_executesql @sql;
