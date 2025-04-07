-- Insert vào bảng Account
INSERT INTO [dbo].[Account] (AccountId, FullName, Email, PasswordHash, PhoneNumber, DateOfBirth, GenderId, AvatarUrl, RoleId, IsVerify, CreatedAt, UpdatedAt, DeletedAt, Status)
VALUES
('admin','admin', 'admin', '$2a$11$CgsAUifZdYkzYxwCQOZJ6eZfO60CCC5lTXG1/ZBGuyxT25JxC5mUe', '0987654321', '1995-05-20', 1, 'https://randomuser.me/api/portraits/men/1.jpg', 1, 1, GETDATE(),NULL, NULL, 1)


-- Insert roles
INSERT INTO Role (RoleName, Note, createdAt, updatedAt) VALUES
('Admin', N'Quản trị viên', GETDATE(), NULL),
('Lessor', N'Chủ nhà', GETDATE(), NULL),
('Lesse', N'Người thuê', GETDATE(), NULL);

-- Insert apartment categories
INSERT INTO AptCategory (CategoryName, Note, createdAt, updatedAt) VALUES
('CC', N'Chung cư', GETDATE(), NULL),
('ND', N'Nhà đất', GETDATE(), NULL),
('CH', N'Căn hộ mini', GETDATE(), NULL),
('KTX', N'Ký túc xá', GETDATE(), NULL),
('KTXV', N'KTX-Vip', GETDATE(), NULL),
('PT', N'Phòng trọ', GETDATE(), NULL);

-- Insert apartment status
INSERT INTO AptStatus (StatusName, Note, createdAt, updatedAt) VALUES
('Available', 'Khả dụng', GETDATE(),NULL),
('Full', 'Đầy', GETDATE(),NULL),
('Maintenance', 'Bảo trì', GETDATE(),NULL),
('Rented', 'Đã thuê', GETDATE(),NULL),
('UnAvailable', 'Không khả dụng', GETDATE(),NULL);

-- Insert transaction types
INSERT INTO OrderType (Name, Note, Month, Amount, createdAt, updatedAt) VALUES
('Silver', N'1 tháng', 1, 100000, GETDATE(),NULL),
('Gold', N'3 tháng', 3, 299000, GETDATE(),NULL),
('Platinum', N'6 tháng', 6, 559000, GETDATE(),NULL),
('Diamond', N'12 tháng', 12, 999000, GETDATE(),NULL);

-- Insert utilities
INSERT INTO Utility (UtilityName, Note, createdAt, updatedAt) VALUES
('air-conditioner', N'Máy lạnh', GETDATE(), NULL),
('kitchen', N'Bếp', GETDATE(), NULL),
('curtain', N'Rèm', GETDATE(), NULL),
('bed', N'Giường', GETDATE(), NULL),
('waterHeater', N'Máy nước nóng', GETDATE(), NULL),
('wifi', N'Wifi', GETDATE(), NULL),
('fridge', N'Tủ lạnh', GETDATE(), NULL),
('washing-machine', N'Máy giặt', GETDATE(), NULL),
('parking', N'Chỗ để xe', GETDATE(), NULL),
('elevator', N'Thang máy', GETDATE(), NULL),
('security', N'Bảo vệ/Camera an ninh', GETDATE(), NULL),
('balcony', N'Ban công', GETDATE(), NULL);
