-- BigInt Software ERP - Role System Tables UPDATE
-- Bu dosya CreateRoleSystemTables.sql'den sonra çalıştırılmalıdır
-- Created: 2024

USE [bigintsoft]
GO

-- Menü yapısını güncelle - Ayarlar alt menülerini ekle
-- Önce mevcut menüleri güncelle (ID'ler değişmiş olabilir)

-- Ayarlar menüsünün ID'sini bul
DECLARE @AyarlarId INT = (SELECT Id FROM [BigIntSoftware].[Menus] WHERE Name = 'Ayarlar' AND ParentId IS NULL)

-- Eğer Ayarlar menüsü yoksa oluştur
IF @AyarlarId IS NULL
BEGIN
    INSERT INTO [BigIntSoftware].[Menus] ([Name], [Description], [Icon], [Controller], [Action], [ParentId], [SortOrder], [IsActive], [IsVisible])
    VALUES ('Ayarlar', 'Sistem ayarları', 'bi-gear', 'Settings', 'Index', NULL, 6, 1, 1)
    
    SET @AyarlarId = SCOPE_IDENTITY()
END

-- Ayarlar alt menülerini ekle (eğer yoksa)
IF NOT EXISTS (SELECT 1 FROM [BigIntSoftware].[Menus] WHERE Name = 'Rol Yönetimi' AND ParentId = @AyarlarId)
BEGIN
    INSERT INTO [BigIntSoftware].[Menus] ([Name], [Description], [Icon], [Controller], [Action], [ParentId], [SortOrder], [IsActive], [IsVisible])
    VALUES ('Rol Yönetimi', 'Rol işlemleri', 'bi-shield-check', 'Role', 'Index', @AyarlarId, 1, 1, 1)
END

IF NOT EXISTS (SELECT 1 FROM [BigIntSoftware].[Menus] WHERE Name = 'Menü Yönetimi' AND ParentId = @AyarlarId)
BEGIN
    INSERT INTO [BigIntSoftware].[Menus] ([Name], [Description], [Icon], [Controller], [Action], [ParentId], [SortOrder], [IsActive], [IsVisible])
    VALUES ('Menü Yönetimi', 'Menü işlemleri', 'bi-list-ul', 'Menu', 'Index', @AyarlarId, 2, 1, 1)
END

IF NOT EXISTS (SELECT 1 FROM [BigIntSoftware].[Menus] WHERE Name = 'Yetki Yönetimi' AND ParentId = @AyarlarId)
BEGIN
    INSERT INTO [BigIntSoftware].[Menus] ([Name], [Description], [Icon], [Controller], [Action], [ParentId], [SortOrder], [IsActive], [IsVisible])
    VALUES ('Yetki Yönetimi', 'Yetki işlemleri', 'bi-key', 'Permission', 'Index', @AyarlarId, 3, 1, 1)
END

-- Ana menülerin sıralamasını güncelle
UPDATE [BigIntSoftware].[Menus] 
SET [SortOrder] = 1 
WHERE Name = 'Dashboard' AND ParentId IS NULL

UPDATE [BigIntSoftware].[Menus] 
SET [SortOrder] = 2 
WHERE Name = 'Kullanıcı Yönetimi' AND ParentId IS NULL

UPDATE [BigIntSoftware].[Menus] 
SET [SortOrder] = 3 
WHERE Name = 'Envanter Yönetimi' AND ParentId IS NULL

UPDATE [BigIntSoftware].[Menus] 
SET [SortOrder] = 4 
WHERE Name = 'Satış Yönetimi' AND ParentId IS NULL

UPDATE [BigIntSoftware].[Menus] 
SET [SortOrder] = 5 
WHERE Name = 'Raporlar' AND ParentId IS NULL

UPDATE [BigIntSoftware].[Menus] 
SET [SortOrder] = 6 
WHERE Name = 'Ayarlar' AND ParentId IS NULL

-- Envanter Yönetimi alt menülerini güncelle
DECLARE @EnvanterId INT = (SELECT Id FROM [BigIntSoftware].[Menus] WHERE Name = 'Envanter Yönetimi' AND ParentId IS NULL)

IF @EnvanterId IS NOT NULL
BEGIN
    -- Mevcut alt menüleri güncelle
    UPDATE [BigIntSoftware].[Menus] 
    SET [ParentId] = @EnvanterId, [SortOrder] = 1
    WHERE Name = 'Ürün Listesi' AND (ParentId IS NULL OR ParentId != @EnvanterId)
    
    UPDATE [BigIntSoftware].[Menus] 
    SET [ParentId] = @EnvanterId, [SortOrder] = 2
    WHERE Name = 'Yeni Ürün' AND (ParentId IS NULL OR ParentId != @EnvanterId)
    
    UPDATE [BigIntSoftware].[Menus] 
    SET [ParentId] = @EnvanterId, [SortOrder] = 3
    WHERE Name = 'Stok Raporları' AND (ParentId IS NULL OR ParentId != @EnvanterId)
END

-- Satış Yönetimi alt menülerini güncelle
DECLARE @SatisId INT = (SELECT Id FROM [BigIntSoftware].[Menus] WHERE Name = 'Satış Yönetimi' AND ParentId IS NULL)

IF @SatisId IS NOT NULL
BEGIN
    -- Mevcut alt menüleri güncelle
    UPDATE [BigIntSoftware].[Menus] 
    SET [ParentId] = @SatisId, [SortOrder] = 1
    WHERE Name = 'Siparişler' AND (ParentId IS NULL OR ParentId != @SatisId)
    
    UPDATE [BigIntSoftware].[Menus] 
    SET [ParentId] = @SatisId, [SortOrder] = 2
    WHERE Name = 'Yeni Sipariş' AND (ParentId IS NULL OR ParentId != @SatisId)
    
    UPDATE [BigIntSoftware].[Menus] 
    SET [ParentId] = @SatisId, [SortOrder] = 3
    WHERE Name = 'Satış Raporları' AND (ParentId IS NULL OR ParentId != @SatisId)
END

-- SuperAdmin rolüne yeni menüleri ata
DECLARE @SuperAdminRoleId INT = (SELECT Id FROM [BigIntSoftware].[Roles] WHERE Name = 'SuperAdmin')
DECLARE @ViewPermissionId INT = (SELECT Id FROM [BigIntSoftware].[Permissions] WHERE Code = 'VIEW')

-- Tüm menülere VIEW yetkisi ver
INSERT INTO [BigIntSoftware].[RolePermissions] ([RoleId], [PermissionId], [MenuId])
SELECT @SuperAdminRoleId, @ViewPermissionId, m.Id
FROM [BigIntSoftware].[Menus] m
WHERE m.IsActive = 1 AND m.IsVisible = 1
AND NOT EXISTS (
    SELECT 1 FROM [BigIntSoftware].[RolePermissions] rp 
    WHERE rp.RoleId = @SuperAdminRoleId 
    AND rp.PermissionId = @ViewPermissionId 
    AND rp.MenuId = m.Id
)

-- Diğer yetkileri de ekle
DECLARE @CreatePermissionId INT = (SELECT Id FROM [BigIntSoftware].[Permissions] WHERE Code = 'CREATE')
DECLARE @EditPermissionId INT = (SELECT Id FROM [BigIntSoftware].[Permissions] WHERE Code = 'EDIT')
DECLARE @DeletePermissionId INT = (SELECT Id FROM [BigIntSoftware].[Permissions] WHERE Code = 'DELETE')
DECLARE @ManagePermissionId INT = (SELECT Id FROM [BigIntSoftware].[Permissions] WHERE Code = 'MANAGE')

-- SuperAdmin'e tüm yetkileri ver
INSERT INTO [BigIntSoftware].[RolePermissions] ([RoleId], [PermissionId], [MenuId])
SELECT @SuperAdminRoleId, p.Id, m.Id
FROM [BigIntSoftware].[Permissions] p
CROSS JOIN [BigIntSoftware].[Menus] m
WHERE m.IsActive = 1 AND m.IsVisible = 1
AND p.IsActive = 1
AND NOT EXISTS (
    SELECT 1 FROM [BigIntSoftware].[RolePermissions] rp 
    WHERE rp.RoleId = @SuperAdminRoleId 
    AND rp.PermissionId = p.Id 
    AND rp.MenuId = m.Id
)

PRINT 'Role System Tables updated successfully!'
PRINT 'Ayarlar menüsü altına Rol, Menü ve Yetki yönetimi eklendi.'
PRINT 'SuperAdmin rolüne tüm menüler için yetkiler atandı.'
