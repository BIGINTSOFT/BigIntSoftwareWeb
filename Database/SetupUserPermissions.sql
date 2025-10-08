-- Kullanıcı 1 için Menü Yetkilerini Kurma
-- Bu script kullanıcı 1'e tüm menülere erişim verir

USE [BigIntSoftware]
GO

-- Önce mevcut menüleri kontrol edelim
SELECT * FROM [BigIntSoftware].[Menus] ORDER BY [SortOrder]

-- Kullanıcı 1'e tüm menülere erişim verelim
-- UserMenus tablosuna kayıt ekleme
INSERT INTO [BigIntSoftware].[UserMenus] ([UserId], [MenuId], [AssignedDate], [IsActive], [AssignedBy], [Notes])
SELECT 
    1 as [UserId],  -- Kullanıcı 1
    [Id] as [MenuId],  -- Menü ID'si
    GETDATE() as [AssignedDate],  -- Atama tarihi
    1 as [IsActive],  -- Aktif
    1 as [AssignedBy],  -- Kendisi tarafından atandı
    'Initial setup - Full access' as [Notes]  -- Not
FROM [BigIntSoftware].[Menus]
WHERE [IsActive] = 1  -- Sadece aktif menüler
GO

-- Kullanıcı 1'e tüm menüler için VIEW izni verelim
INSERT INTO [BigIntSoftware].[UserMenuPermissions] ([UserId], [MenuId], [PermissionId], [PermissionLevel], [AssignedDate], [IsActive], [AssignedBy], [Notes])
SELECT 
    1 as [UserId],  -- Kullanıcı 1
    m.[Id] as [MenuId],  -- Menü ID'si
    1 as [PermissionId],  -- VIEW izni (Permission ID 1)
    'VIEW' as [PermissionLevel],  -- İzin seviyesi
    GETDATE() as [AssignedDate],  -- Atama tarihi
    1 as [IsActive],  -- Aktif
    1 as [AssignedBy],  -- Kendisi tarafından atandı
    'Initial setup - View permission' as [Notes]  -- Not
FROM [BigIntSoftware].[Menus] m
WHERE m.[IsActive] = 1  -- Sadece aktif menüler
GO

-- Kullanıcı 1'e tüm menüler için CREATE izni verelim
INSERT INTO [BigIntSoftware].[UserMenuPermissions] ([UserId], [MenuId], [PermissionId], [PermissionLevel], [AssignedDate], [IsActive], [AssignedBy], [Notes])
SELECT 
    1 as [UserId],  -- Kullanıcı 1
    m.[Id] as [MenuId],  -- Menü ID'si
    2 as [PermissionId],  -- CREATE izni (Permission ID 2)
    'CREATE' as [PermissionLevel],  -- İzin seviyesi
    GETDATE() as [AssignedDate],  -- Atama tarihi
    1 as [IsActive],  -- Aktif
    1 as [AssignedBy],  -- Kendisi tarafından atandı
    'Initial setup - Create permission' as [Notes]  -- Not
FROM [BigIntSoftware].[Menus] m
WHERE m.[IsActive] = 1  -- Sadece aktif menüler
GO

-- Kullanıcı 1'e tüm menüler için EDIT izni verelim
INSERT INTO [BigIntSoftware].[UserMenuPermissions] ([UserId], [MenuId], [PermissionId], [PermissionLevel], [AssignedDate], [IsActive], [AssignedBy], [Notes])
SELECT 
    1 as [UserId],  -- Kullanıcı 1
    m.[Id] as [MenuId],  -- Menü ID'si
    3 as [PermissionId],  -- EDIT izni (Permission ID 3)
    'EDIT' as [PermissionLevel],  -- İzin seviyesi
    GETDATE() as [AssignedDate],  -- Atama tarihi
    1 as [IsActive],  -- Aktif
    1 as [AssignedBy],  -- Kendisi tarafından atandı
    'Initial setup - Edit permission' as [Notes]  -- Not
FROM [BigIntSoftware].[Menus] m
WHERE m.[IsActive] = 1  -- Sadece aktif menüler
GO

-- Kullanıcı 1'e tüm menüler için DELETE izni verelim
INSERT INTO [BigIntSoftware].[UserMenuPermissions] ([UserId], [MenuId], [PermissionId], [PermissionLevel], [AssignedDate], [IsActive], [AssignedBy], [Notes])
SELECT 
    1 as [UserId],  -- Kullanıcı 1
    m.[Id] as [MenuId],  -- Menü ID'si
    4 as [PermissionId],  -- DELETE izni (Permission ID 4)
    'DELETE' as [PermissionLevel],  -- İzin seviyesi
    GETDATE() as [AssignedDate],  -- Atama tarihi
    1 as [IsActive],  -- Aktif
    1 as [AssignedBy],  -- Kendisi tarafından atandı
    'Initial setup - Delete permission' as [Notes]  -- Not
FROM [BigIntSoftware].[Menus] m
WHERE m.[IsActive] = 1  -- Sadece aktif menüler
GO

-- Kontrol sorguları - Eklenen kayıtları görelim
PRINT '=== Kullanıcı 1 Menü Erişimleri ==='
SELECT 
    um.[Id],
    um.[UserId],
    m.[Name] as [MenuName],
    um.[AssignedDate],
    um.[IsActive],
    um.[Notes]
FROM [BigIntSoftware].[UserMenus] um
INNER JOIN [BigIntSoftware].[Menus] m ON um.[MenuId] = m.[Id]
WHERE um.[UserId] = 1
ORDER BY m.[SortOrder]

PRINT '=== Kullanıcı 1 Menü İzinleri ==='
SELECT 
    ump.[Id],
    ump.[UserId],
    m.[Name] as [MenuName],
    p.[Name] as [PermissionName],
    ump.[PermissionLevel],
    ump.[AssignedDate],
    ump.[IsActive],
    ump.[Notes]
FROM [BigIntSoftware].[UserMenuPermissions] ump
INNER JOIN [BigIntSoftware].[Menus] m ON ump.[MenuId] = m.[Id]
INNER JOIN [BigIntSoftware].[Permissions] p ON ump.[PermissionId] = p.[Id]
WHERE ump.[UserId] = 1
ORDER BY m.[SortOrder], ump.[PermissionLevel]

PRINT '=== Toplam Kayıt Sayıları ==='
SELECT 
    'UserMenus' as [TableName],
    COUNT(*) as [RecordCount]
FROM [BigIntSoftware].[UserMenus]
WHERE [UserId] = 1

UNION ALL

SELECT 
    'UserMenuPermissions' as [TableName],
    COUNT(*) as [RecordCount]
FROM [BigIntSoftware].[UserMenuPermissions]
WHERE [UserId] = 1

UNION ALL

SELECT 
    'Total Menus' as [TableName],
    COUNT(*) as [RecordCount]
FROM [BigIntSoftware].[Menus]
WHERE [IsActive] = 1

UNION ALL

SELECT 
    'Total Permissions' as [TableName],
    COUNT(*) as [RecordCount]
FROM [BigIntSoftware].[Permissions]
WHERE [IsActive] = 1
GO

PRINT 'Kullanıcı 1 için menü yetkileri başarıyla kuruldu!'
PRINT 'Artık login olduktan sonra sol menüde tüm menüler görünecek.'
