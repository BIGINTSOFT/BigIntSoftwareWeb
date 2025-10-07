-- =============================================
-- ERP Standartlarına Uygun Rol ve Yetki Sistemi
-- Varsayılan Veri Ekleme Script'i
-- =============================================

USE [BigIntSoftwareDB]
GO

-- =============================================
-- 1. VARSayılan ROLLER
-- =============================================

-- Sistem Yöneticisi Rolü
IF NOT EXISTS (SELECT 1 FROM [BigIntSoftware].[Roles] WHERE [Name] = 'Sistem Yöneticisi')
BEGIN
    INSERT INTO [BigIntSoftware].[Roles] ([Name], [Description], [IsActive], [CreatedDate])
    VALUES ('Sistem Yöneticisi', 'Tüm sistem yetkilerine sahip yönetici rolü', 1, GETDATE())
END
GO

-- Muhasebe Uzmanı Rolü
IF NOT EXISTS (SELECT 1 FROM [BigIntSoftware].[Roles] WHERE [Name] = 'Muhasebe Uzmanı')
BEGIN
    INSERT INTO [BigIntSoftware].[Roles] ([Name], [Description], [IsActive], [CreatedDate])
    VALUES ('Muhasebe Uzmanı', 'Muhasebe işlemlerini yöneten uzman rolü', 1, GETDATE())
END
GO

-- Satış Temsilcisi Rolü
IF NOT EXISTS (SELECT 1 FROM [BigIntSoftware].[Roles] WHERE [Name] = 'Satış Temsilcisi')
BEGIN
    INSERT INTO [BigIntSoftware].[Roles] ([Name], [Description], [IsActive], [CreatedDate])
    VALUES ('Satış Temsilcisi', 'Satış işlemlerini yöneten temsilci rolü', 1, GETDATE())
END
GO

-- İnsan Kaynakları Uzmanı Rolü
IF NOT EXISTS (SELECT 1 FROM [BigIntSoftware].[Roles] WHERE [Name] = 'İnsan Kaynakları Uzmanı')
BEGIN
    INSERT INTO [BigIntSoftware].[Roles] ([Name], [Description], [IsActive], [CreatedDate])
    VALUES ('İnsan Kaynakları Uzmanı', 'Personel işlemlerini yöneten uzman rolü', 1, GETDATE())
END
GO

-- Raporlama Uzmanı Rolü
IF NOT EXISTS (SELECT 1 FROM [BigIntSoftware].[Roles] WHERE [Name] = 'Raporlama Uzmanı')
BEGIN
    INSERT INTO [BigIntSoftware].[Roles] ([Name], [Description], [IsActive], [CreatedDate])
    VALUES ('Raporlama Uzmanı', 'Sistem raporlarını görüntüleyen uzman rolü', 1, GETDATE())
END
GO

-- =============================================
-- 2. VARSayılan MENÜLER (Eğer yoksa)
-- =============================================

-- Ana Menüler
IF NOT EXISTS (SELECT 1 FROM [BigIntSoftware].[Menus] WHERE [Name] = 'Kullanıcı Yönetimi')
BEGIN
    INSERT INTO [BigIntSoftware].[Menus] ([Name], [Description], [Icon], [Controller], [Action], [ParentId], [SortOrder], [IsActive], [IsVisible], [CreatedDate])
    VALUES ('Kullanıcı Yönetimi', 'Sistem kullanıcılarını yönetme', 'bi bi-people', 'User', 'Index', NULL, 1, 1, 1, GETDATE())
END
GO

IF NOT EXISTS (SELECT 1 FROM [BigIntSoftware].[Menus] WHERE [Name] = 'Rol Yönetimi')
BEGIN
    INSERT INTO [BigIntSoftware].[Menus] ([Name], [Description], [Icon], [Controller'], [Action], [ParentId], [SortOrder], [IsActive], [IsVisible], [CreatedDate])
    VALUES ('Rol Yönetimi', 'Sistem rollerini yönetme', 'bi bi-shield-check', 'Role', 'Index', NULL, 2, 1, 1, GETDATE())
END
GO

IF NOT EXISTS (SELECT 1 FROM [BigIntSoftware].[Menus] WHERE [Name] = 'Menü Yönetimi')
BEGIN
    INSERT INTO [BigIntSoftware].[Menus] ([Name], [Description], [Icon], [Controller], [Action], [ParentId], [SortOrder], [IsActive], [IsVisible], [CreatedDate])
    VALUES ('Menü Yönetimi', 'Sistem menülerini yönetme', 'bi bi-list-ul', 'Menu', 'Index', NULL, 3, 1, 1, GETDATE())
END
GO

IF NOT EXISTS (SELECT 1 FROM [BigIntSoftware].[Menus] WHERE [Name] = 'Yetki Yönetimi')
BEGIN
    INSERT INTO [BigIntSoftware].[Menus] ([Name], [Description], [Icon], [Controller], [Action], [ParentId], [SortOrder], [IsActive], [IsVisible], [CreatedDate])
    VALUES ('Yetki Yönetimi', 'Sistem yetkilerini yönetme', 'bi bi-gear', 'Permission', 'Index', NULL, 4, 1, 1, GETDATE())
END
GO

-- =============================================
-- 3. ROL-MENÜ-YETKİ İLİŞKİLERİ
-- =============================================

-- Sistem Yöneticisi: Tüm menülerde tüm yetkiler
DECLARE @SistemYoneticisiId INT = (SELECT [Id] FROM [BigIntSoftware].[Roles] WHERE [Name] = 'Sistem Yöneticisi')

-- Kullanıcı Yönetimi menüsü için Sistem Yöneticisi yetkileri
DECLARE @KullaniciYonetimiId INT = (SELECT [Id] FROM [BigIntSoftware].[Menus] WHERE [Name] = 'Kullanıcı Yönetimi')
IF @KullaniciYonetimiId IS NOT NULL
BEGIN
    INSERT INTO [BigIntSoftware].[RoleMenuPermissions] ([RoleId], [MenuId], [PermissionLevel], [AssignedDate], [IsActive])
    VALUES (@SistemYoneticisiId, @KullaniciYonetimiId, 'DELETE', GETDATE(), 1)
END

-- Rol Yönetimi menüsü için Sistem Yöneticisi yetkileri
DECLARE @RolYonetimiId INT = (SELECT [Id] FROM [BigIntSoftware].[Menus] WHERE [Name] = 'Rol Yönetimi')
IF @RolYonetimiId IS NOT NULL
BEGIN
    INSERT INTO [BigIntSoftware].[RoleMenuPermissions] ([RoleId], [MenuId], [PermissionLevel], [AssignedDate], [IsActive])
    VALUES (@SistemYoneticisiId, @RolYonetimiId, 'DELETE', GETDATE(), 1)
END

-- Menü Yönetimi menüsü için Sistem Yöneticisi yetkileri
DECLARE @MenuYonetimiId INT = (SELECT [Id] FROM [BigIntSoftware].[Menus] WHERE [Name] = 'Menü Yönetimi')
IF @MenuYonetimiId IS NOT NULL
BEGIN
    INSERT INTO [BigIntSoftware].[RoleMenuPermissions] ([RoleId], [MenuId], [PermissionLevel], [AssignedDate], [IsActive])
    VALUES (@SistemYoneticisiId, @MenuYonetimiId, 'DELETE', GETDATE(), 1)
END

-- Yetki Yönetimi menüsü için Sistem Yöneticisi yetkileri
DECLARE @YetkiYonetimiId INT = (SELECT [Id] FROM [BigIntSoftware].[Menus] WHERE [Name] = 'Yetki Yönetimi')
IF @YetkiYonetimiId IS NOT NULL
BEGIN
    INSERT INTO [BigIntSoftware].[RoleMenuPermissions] ([RoleId], [MenuId], [PermissionLevel], [AssignedDate], [IsActive])
    VALUES (@SistemYoneticisiId, @YetkiYonetimiId, 'DELETE', GETDATE(), 1)
END

-- =============================================
-- 4. VARSayılan KULLANICI-ROL İLİŞKİLERİ
-- =============================================

-- Admin kullanıcısını Sistem Yöneticisi rolüne ata (eğer admin kullanıcısı varsa)
DECLARE @AdminUserId INT = (SELECT TOP 1 [Id] FROM [BigIntSoftware].[Users] WHERE [Username] = 'admin' OR [Username] = 'Admin')
IF @AdminUserId IS NOT NULL
BEGIN
    INSERT INTO [BigIntSoftware].[UserRoles] ([UserId], [RoleId], [AssignedDate], [IsActive])
    VALUES (@AdminUserId, @SistemYoneticisiId, GETDATE(), 1)
END

-- =============================================
-- 5. BAŞARILI MESAJI
-- =============================================
PRINT 'ERP Standartlarına Uygun Rol ve Yetki Sistemi varsayılan verileri başarıyla eklendi!'
PRINT 'Eklenen Veriler:'
PRINT '- 5 Varsayılan Rol'
PRINT '- 4 Ana Menü'
PRINT '- Rol-Menü-Yetki İlişkileri'
PRINT '- Admin kullanıcısı Sistem Yöneticisi rolüne atandı'
GO
