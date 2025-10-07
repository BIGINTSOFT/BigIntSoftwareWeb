-- =============================================
-- 1 Numaralı Kullanıcı İçin Yetki Kurulumu
-- Layout'ta menüleri gösterebilmek için gerekli veriler
-- =============================================

USE [BigIntSoftwareDB]
GO

-- =============================================
-- 1. KULLANICI VAR MI KONTROL ET
-- =============================================
IF NOT EXISTS (SELECT 1 FROM [BigIntSoftware].[Users] WHERE [Id] = 1)
BEGIN
    PRINT 'HATA: 1 numaralı kullanıcı bulunamadı!'
    PRINT 'Önce kullanıcıyı oluşturun.'
    RETURN
END
GO

-- =============================================
-- 2. SİSTEM YÖNETİCİSİ ROLÜNÜ KONTROL ET
-- =============================================
DECLARE @SistemYoneticisiId INT
SELECT @SistemYoneticisiId = [Id] FROM [BigIntSoftware].[Roles] WHERE [Name] = 'Sistem Yöneticisi'

IF @SistemYoneticisiId IS NULL
BEGIN
    PRINT 'HATA: Sistem Yöneticisi rolü bulunamadı!'
    PRINT 'Önce InsertERPRoleSystemData.sql scriptini çalıştırın.'
    RETURN
END

PRINT 'Sistem Yöneticisi Rol ID: ' + CAST(@SistemYoneticisiId AS VARCHAR(10))
GO

-- =============================================
-- 3. KULLANICIYI SİSTEM YÖNETİCİSİ ROLÜNE ATA
-- =============================================
DECLARE @SistemYoneticisiId INT
SELECT @SistemYoneticisiId = [Id] FROM [BigIntSoftware].[Roles] WHERE [Name] = 'Sistem Yöneticisi'

-- Kullanıcı zaten bu role atanmış mı kontrol et
IF NOT EXISTS (SELECT 1 FROM [BigIntSoftware].[UserRoles] WHERE [UserId] = 1 AND [RoleId] = @SistemYoneticisiId)
BEGIN
    INSERT INTO [BigIntSoftware].[UserRoles] ([UserId], [RoleId], [AssignedDate], [IsActive], [Notes])
    VALUES (1, @SistemYoneticisiId, GETDATE(), 1, 'Sistem kurulumu - Otomatik atama')
    
    PRINT '1 numaralı kullanıcı Sistem Yöneticisi rolüne atandı.'
END
ELSE
BEGIN
    PRINT '1 numaralı kullanıcı zaten Sistem Yöneticisi rolüne atanmış.'
END
GO

-- =============================================
-- 4. MENÜLERİ KONTROL ET VE EKLE
-- =============================================

-- Ana Menüler
IF NOT EXISTS (SELECT 1 FROM [BigIntSoftware].[Menus] WHERE [Name] = 'Ana Sayfa')
BEGIN
    INSERT INTO [BigIntSoftware].[Menus] ([Name], [Description], [Icon], [Controller], [Action], [ParentId], [SortOrder], [IsActive], [IsVisible], [CreatedDate])
    VALUES ('Ana Sayfa', 'Sistem ana sayfası', 'bi bi-house', 'Home', 'Index', NULL, 1, 1, 1, GETDATE())
END

IF NOT EXISTS (SELECT 1 FROM [BigIntSoftware].[Menus] WHERE [Name] = 'Kullanıcı Yönetimi')
BEGIN
    INSERT INTO [BigIntSoftware].[Menus] ([Name], [Description], [Icon], [Controller], [Action], [ParentId], [SortOrder], [IsActive], [IsVisible], [CreatedDate])
    VALUES ('Kullanıcı Yönetimi', 'Sistem kullanıcılarını yönetme', 'bi bi-people', 'User', 'Index', NULL, 2, 1, 1, GETDATE())
END

IF NOT EXISTS (SELECT 1 FROM [BigIntSoftware].[Menus] WHERE [Name] = 'Rol Yönetimi')
BEGIN
    INSERT INTO [BigIntSoftware].[Menus] ([Name], [Description], [Icon], [Controller], [Action], [ParentId], [SortOrder], [IsActive], [IsVisible], [CreatedDate])
    VALUES ('Rol Yönetimi', 'Sistem rollerini yönetme', 'bi bi-shield-check', 'Role', 'Index', NULL, 3, 1, 1, GETDATE())
END

IF NOT EXISTS (SELECT 1 FROM [BigIntSoftware].[Menus] WHERE [Name] = 'Menü Yönetimi')
BEGIN
    INSERT INTO [BigIntSoftware].[Menus] ([Name], [Description], [Icon], [Controller], [Action], [ParentId], [SortOrder], [IsActive], [IsVisible], [CreatedDate])
    VALUES ('Menü Yönetimi', 'Sistem menülerini yönetme', 'bi bi-list-ul', 'Menu', 'Index', NULL, 4, 1, 1, GETDATE())
END

IF NOT EXISTS (SELECT 1 FROM [BigIntSoftware].[Menus] WHERE [Name] = 'Yetki Yönetimi')
BEGIN
    INSERT INTO [BigIntSoftware].[Menus] ([Name], [Description], [Icon], [Controller], [Action], [ParentId], [SortOrder], [IsActive], [IsVisible], [CreatedDate])
    VALUES ('Yetki Yönetimi', 'Sistem yetkilerini yönetme', 'bi bi-gear', 'Permission', 'Index', NULL, 5, 1, 1, GETDATE())
END

IF NOT EXISTS (SELECT 1 FROM [BigIntSoftware].[Menus] WHERE [Name] = 'Ayarlar')
BEGIN
    INSERT INTO [BigIntSoftware].[Menus] ([Name], [Description], [Icon], [Controller], [Action], [ParentId], [SortOrder], [IsActive], [IsVisible], [CreatedDate])
    VALUES ('Ayarlar', 'Sistem ayarları', 'bi bi-gear-fill', 'Settings', 'Index', NULL, 6, 1, 1, GETDATE())
END

PRINT 'Menüler kontrol edildi ve eklendi.'
GO

-- =============================================
-- 5. SİSTEM YÖNETİCİSİ ROLÜNE TÜM MENÜLERDE TÜM YETKİLERİ VER
-- =============================================
DECLARE @SistemYoneticisiId INT
SELECT @SistemYoneticisiId = [Id] FROM [BigIntSoftware].[Roles] WHERE [Name] = 'Sistem Yöneticisi'

-- Tüm menüleri al
DECLARE @MenuId INT
DECLARE menu_cursor CURSOR FOR 
SELECT [Id] FROM [BigIntSoftware].[Menus] WHERE [IsActive] = 1

OPEN menu_cursor
FETCH NEXT FROM menu_cursor INTO @MenuId

WHILE @@FETCH_STATUS = 0
BEGIN
    -- VIEW yetkisi ver (eğer yoksa)
    IF NOT EXISTS (SELECT 1 FROM [BigIntSoftware].[RoleMenuPermissions] WHERE [RoleId] = @SistemYoneticisiId AND [MenuId] = @MenuId)
    BEGIN
        INSERT INTO [BigIntSoftware].[RoleMenuPermissions] ([RoleId], [MenuId], [PermissionLevel], [AssignedDate], [IsActive], [Notes])
        VALUES (@SistemYoneticisiId, @MenuId, 'DELETE', GETDATE(), 1, 'Sistem kurulumu - Tüm yetkiler')
        
        PRINT 'Sistem Yöneticisi rolüne ' + CAST(@MenuId AS VARCHAR(10)) + ' numaralı menü için DELETE yetkisi verildi.'
    END
    
    FETCH NEXT FROM menu_cursor INTO @MenuId
END

CLOSE menu_cursor
DEALLOCATE menu_cursor

PRINT 'Sistem Yöneticisi rolüne tüm menülerde tüm yetkiler verildi.'
GO

-- =============================================
-- 6. KONTROL SORGULARI
-- =============================================

PRINT '=== KONTROL SORGULARI ==='

-- Kullanıcının rolleri
PRINT '1 numaralı kullanıcının rolleri:'
SELECT r.[Name] as 'Rol Adı', ur.[AssignedDate] as 'Atama Tarihi', ur.[IsActive] as 'Aktif'
FROM [BigIntSoftware].[UserRoles] ur
INNER JOIN [BigIntSoftware].[Roles] r ON ur.[RoleId] = r.[Id]
WHERE ur.[UserId] = 1

-- Kullanıcının erişebileceği menüler
PRINT '1 numaralı kullanıcının erişebileceği menüler:'
SELECT DISTINCT m.[Name] as 'Menü Adı', rmp.[PermissionLevel] as 'Yetki Seviyesi'
FROM [BigIntSoftware].[UserRoles] ur
INNER JOIN [BigIntSoftware].[RoleMenuPermissions] rmp ON ur.[RoleId] = rmp.[RoleId]
INNER JOIN [BigIntSoftware].[Menus] m ON rmp.[MenuId] = m.[Id]
WHERE ur.[UserId] = 1 AND ur.[IsActive] = 1 AND rmp.[IsActive] = 1 AND m.[IsActive] = 1 AND m.[IsVisible] = 1
ORDER BY m.[SortOrder]

PRINT '=== KURULUM TAMAMLANDI ==='
PRINT '1 numaralı kullanıcı artık tüm menülere erişebilir.'
PRINT 'Layout''ta menüler görünecektir.'
GO
