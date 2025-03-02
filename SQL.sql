Create Database RentEase

-- Bảng phụ
CREATE TABLE Utility (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UtilityName NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(255) NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NULL,
	DeletedAt DATETIME NULL,
	Status Bit DEFAULT 1 
);
CREATE TABLE AptCategory (
    Id INT PRIMARY KEY IDENTITY(1,1),
    CategoryName NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(255) NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NULL,
	DeletedAt DATETIME NULL,
	Status Bit DEFAULT 1 
);
CREATE TABLE AptStatus (
    Id INT PRIMARY KEY IDENTITY(1,1),
    StatusName NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(255) NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NULL,
	DeletedAt DATETIME NULL,
	Status Bit DEFAULT 1 
);
CREATE TABLE Role (
    Id INT PRIMARY KEY IDENTITY(1,1),
    RoleName NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(255) NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NULL,
	DeletedAt DATETIME NULL,
	Status Bit DEFAULT 1 
);
CREATE TABLE TransactionType (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TypeName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255) NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NULL,
	DeletedAt DATETIME NULL,
	Status Bit DEFAULT 1 
);

-- Bảng chính
CREATE TABLE Account (
    Id INT PRIMARY KEY IDENTITY(1,1),
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
    Id INT PRIMARY KEY IDENTITY(1,1),
    OwnerId INT NOT NULL,
	AptCode NVARCHAR(10) NOT NULL,
    Name NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX) NULL,
	Area FLOAT DEFAULT 0,
    Address NVARCHAR(500) NOT NULL,
	AddressLink NVARCHAR(500) NULL,
    ProvinceId INT NOT NULL,
    DistrictId INT NOT NULL,
	WardId INT NOT NULL,
    RentPrice BIGINT NOT NULL,
    PilePrice BIGINT NULL,
    CategoryId INT NOT NULL,
    StatusId INT NOT NULL,
    NumberOfRoom INT NOT NULL,
    AvailableRoom INT NOT NULL,
    ApproveStatusId INT NOT NULL,
    Note NVARCHAR(MAX) NULL,
    Rating FLOAT DEFAULT 0,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NULL,
	DeletedAt DATETIME NULL,
	Status Bit DEFAULT 1 
    CONSTRAINT FK_Apt_AptCategory FOREIGN KEY (CategoryId) REFERENCES AptCategory(Id),
    CONSTRAINT FK_Apt_AptStatus FOREIGN KEY (StatusId) REFERENCES AptStatus(Id)
);
CREATE INDEX IX_Apt_OwnerId ON Apt (OwnerId);
CREATE INDEX IX_Apt_AptCategory ON Apt (CategoryId);
CREATE INDEX IX_Apt_AptStatus ON Apt (StatusId);


CREATE TABLE AccountVerification (
    Id INT PRIMARY KEY IDENTITY(1,1),
    AccountId INT NOT NULL,
    VerificationCode NVARCHAR(50) NOT NULL,
    ExpiresAt DATETIME NOT NULL,
    IsUsed BIT DEFAULT 0,
    CreatedAt DATETIME NOT NULL,
    CONSTRAINT FK_AccountVerification_Account FOREIGN KEY (AccountId) REFERENCES Account(Id)
);


CREATE TABLE AptUtility (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    AptId INT NOT NULL,
    UtilityId INT NOT NULL,
	Description NVARCHAR(500) NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NULL,
	DeletedAt DATETIME NULL,
	Status Bit DEFAULT 1 
    CONSTRAINT FK_AptUtility_Apt FOREIGN KEY (AptId) REFERENCES Apt(Id) ON DELETE CASCADE,
    CONSTRAINT FK_AptUtility_Utility FOREIGN KEY (UtilityId) REFERENCES Utility(Id) ON DELETE CASCADE,
    CONSTRAINT UQ_AptUtility UNIQUE (AptId, UtilityId) 
);
CREATE INDEX IX_AptUtility_AptId ON AptUtility(AptId);
CREATE INDEX IX_AptUtility_UtilityId ON AptUtility(UtilityId);


CREATE TABLE MaintenanceRequest (
    Id INT PRIMARY KEY IDENTITY(1,1),
    AptId INT NOT NULL,
    LesseeId INT NOT NULL,
    Description NVARCHAR(500) NULL,
    Priority INT NOT NULL,
    AgentId INT NOT NULL,
    ApproveStatusId INT NOT NULL,
    ProgressStatusId INT NOT NULL,
    Note NVARCHAR(500) NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NULL,
	DeletedAt DATETIME NULL,
	Status Bit DEFAULT 1 
    CONSTRAINT FK_MaintenanceRequest_Apt FOREIGN KEY (AptId) REFERENCES Apt(Id),
    CONSTRAINT FK_MaintenanceRequest_Lessee FOREIGN KEY (LesseeId) REFERENCES Account(Id),
    CONSTRAINT FK_MaintenanceRequest_Agent FOREIGN KEY (AgentId) REFERENCES Account(Id)
);
CREATE INDEX IX_MaintenanceRequest_AptId ON MaintenanceRequest (AptId);
CREATE INDEX IX_MaintenanceRequest_LesseeId ON MaintenanceRequest (LesseeId);


CREATE TABLE Review (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ReviewerId INT NOT NULL,
    AptId INT NOT NULL,
    Rating FLOAT DEFAULT 0,
    Comment NVARCHAR(500) NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NULL,
	DeletedAt DATETIME NULL,
	Status Bit DEFAULT 1 
    CONSTRAINT FK_Review_Reviewer FOREIGN KEY (ReviewerId) REFERENCES Account(Id),
    CONSTRAINT FK_Review_Apt FOREIGN KEY (AptId) REFERENCES Apt(Id)
);
CREATE INDEX IX_Review_AptId ON Review (AptId);
CREATE INDEX IX_Review_ReviewerId ON Review (ReviewerId);


CREATE TABLE CurrentResident (
    Id INT PRIMARY KEY IDENTITY(1,1),
    AptId INT NOT NULL,
    AccountId INT NOT NULL,
    MoveInDate DATETIME NOT NULL,
    MoveOutDate DATETIME NULL,
    StatusId INT NOT NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NULL,
	DeletedAt DATETIME NULL,
	Status Bit DEFAULT 1 
    CONSTRAINT FK_CurrentResident_Apt FOREIGN KEY (AptId) REFERENCES Apt(Id),
    CONSTRAINT FK_CurrentResident_Account FOREIGN KEY (AccountId) REFERENCES Account(Id)
);
CREATE INDEX IX_CurrentResident_AptId ON CurrentResident (AptId);
CREATE INDEX IX_CurrentResident_AccountId ON CurrentResident (AccountId);


CREATE TABLE AptImage (
    Id INT PRIMARY KEY IDENTITY(1,1),
    AptId INT NOT NULL,
    ImageURL1 NVARCHAR(max) NULL,
    ImageURL2 NVARCHAR(max) NULL,
    ImageURL3 NVARCHAR(max) NULL,
	ImageURL4 NVARCHAR(max) NULL,
	ImageURL5 NVARCHAR(max) NULL,
	ImageURL6 NVARCHAR(max) NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NULL,
	DeletedAt DATETIME NULL,
	Status Bit DEFAULT 1 
    CONSTRAINT FK_AptImage_Apt FOREIGN KEY (AptId) REFERENCES Apt(Id)
);


CREATE TABLE Contract (
    Id INT PRIMARY KEY IDENTITY(1,1),
    AptId INT NOT NULL,
    LessorId INT NOT NULL,
    LesseeId INT NOT NULL,
    AgentId INT NULL,
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    RentPrice BIGINT NOT NULL,
    PilePrice BIGINT NOT NULL,
    FileURL NVARCHAR(MAX) NULL,
    ContractStatusId INT NOT NULL,
    ApproveStatusId INT NOT NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NULL,
	DeletedAt DATETIME NULL,
	Status Bit DEFAULT 1 
    CONSTRAINT FK_Contract_Apt FOREIGN KEY (AptId) REFERENCES Apt(Id),
    CONSTRAINT FK_Contract_Lessor FOREIGN KEY (LessorId) REFERENCES Account(Id),
    CONSTRAINT FK_Contract_Lessee FOREIGN KEY (LesseeId) REFERENCES Account(Id),
    CONSTRAINT FK_Contract_Agent FOREIGN KEY (AgentId) REFERENCES Account(Id)
);
CREATE INDEX IX_Contract_AptId ON Contract (AptId);
CREATE INDEX IX_Contract_LessorId ON Contract (LessorId);
CREATE INDEX IX_Contract_LesseeId ON Contract (LesseeId);
CREATE INDEX IX_Contract_AgentId ON Contract (AgentId);
CREATE INDEX IX_Contract_ContractStatusId ON Contract (ContractStatusId);
CREATE INDEX IX_Contract_ApproveStatusId ON Contract (ApproveStatusId);


CREATE TABLE AccountToken (
    Id INT PRIMARY KEY IDENTITY(1,1),
    AccountId INT NOT NULL,
    RefreshToken NVARCHAR(MAX) NOT NULL,
    ExpiresAt DATETIME NOT NULL,
    CreatedAt DATETIME NOT NULL,
    CONSTRAINT FK_AccountToken_Account FOREIGN KEY (AccountId) REFERENCES Account(Id)
);
CREATE INDEX IX_AccountToken_AccountId ON AccountToken (AccountId);

CREATE TABLE Orders (
    Id NVARCHAR(100) PRIMARY KEY,
    ContractId INT NOT NULL,
    LessorId INT NOT NULL,
    LesseeId INT NOT NULL,
	Amount DECIMAL(18,2) NOT NULL,
    TransactionTypeId INT NOT NULL,
    TransactionStatusId INT NOT NULL,
	DueDate DATETIME NOT NULL,
    CreatedAt DATETIME NOT NULL,
    CONSTRAINT FK_Order_Contract FOREIGN KEY (ContractId) REFERENCES Contract(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Order_Lessor FOREIGN KEY (LessorId) REFERENCES Account(Id),
    CONSTRAINT FK_Order_Lessee FOREIGN KEY (LesseeId) REFERENCES Account(Id),
    CONSTRAINT FK_Order_TransactionType FOREIGN KEY (TransactionTypeId) REFERENCES TransactionType(Id) ON DELETE CASCADE,

);

CREATE TABLE Wallet (
    AccountId INT NOT NULL PRIMARY KEY,
    Balance DECIMAL(18,2) NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NULL,
	DeletedAt DATETIME NULL,
	Status Bit DEFAULT 1 
    CONSTRAINT FK_Wallet_Account FOREIGN KEY (AccountId) REFERENCES dbo.Account(Id) ON DELETE CASCADE
);
CREATE INDEX IDX_Wallet_AccountId ON Wallet (AccountId);


CREATE TABLE WalletTransaction (
    Id INT IDENTITY(1,1) PRIMARY KEY,
	OrderId NVARCHAR(100) NULL UNIQUE,
    WalletId INT NOT NULL, 
    Amount DECIMAL(18,2) NOT NULL,
    TransactionTypeId INT NOT NULL,
    TransactionStatusId INT NOT NULL,
    Description NVARCHAR(500) NULL,
    CreatedAt DATETIME NOT NULL,
	CONSTRAINT FK_WalletTransaction_Orders FOREIGN KEY (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE,
    CONSTRAINT FK_WalletTransaction_Wallet FOREIGN KEY (WalletId) REFERENCES Wallet(AccountId) ON DELETE CASCADE,
    CONSTRAINT FK_WalletTransaction_TransactionType FOREIGN KEY (TransactionTypeId) REFERENCES TransactionType(Id)
);
CREATE INDEX IDX_WalletTransaction_WalletId ON WalletTransaction (WalletId);
CREATE INDEX IDX_WalletTransaction_TransactionTypeId ON WalletTransaction (TransactionTypeId);



-- Xóa các bảng có khóa ngoại trước
DROP TABLE IF EXISTS Role;
DROP TABLE IF EXISTS Utility;
DROP TABLE IF EXISTS AptCategory;
DROP TABLE IF EXISTS AptStatus;
DROP TABLE IF EXISTS TransactionType;

DROP TABLE IF EXISTS Orders;

DROP TABLE IF EXISTS AccountVerification;
DROP TABLE IF EXISTS AccountToken;
DROP TABLE IF EXISTS WalletTransactions;
DROP TABLE IF EXISTS Wallet;
DROP TABLE IF EXISTS AptUtility;
DROP TABLE IF EXISTS AptImage;
DROP TABLE IF EXISTS CurrentResident;
DROP TABLE IF EXISTS Review;
DROP TABLE IF EXISTS Contract;
DROP TABLE IF EXISTS ApproveStatus;
DROP TABLE IF EXISTS MaintenanceRequest;

DROP TABLE IF EXISTS Apt;
DROP TABLE IF EXISTS Account;