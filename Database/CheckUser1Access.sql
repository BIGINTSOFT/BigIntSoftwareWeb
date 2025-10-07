-- =============================================
-- 1 Numaralı Kullanıcının Erişim Durumunu Kontrol Et
-- =============================================

USE [BigIntSoftwareDB]
GO

PRINT '=== 1 NUMARALI KULLANICI ERİŞİM KONTROLÜ ==='
PRINT ''

-- =============================================
-- 1. KULLANICI BİLGİLERİ
-- =============================================
PRINT '1. KULLANICI BİLGİLERİ:'
IF EXISTS (SELECT 1 FROM [BigIntSoftware].[Users] WHERE [Id] = 1)
BEGIN
    SELECT 
        [Id],
        [Username],
        [Email],
        [FirstName],
        [LastName],
        [IsActive],
        [CreatedDate]
    FROM [BigIntSoftware].[Users] 
    WHERE [Id] = 1
END
ELSE
BEGIN
    PRINT 'HATA: 1 numaralı kullanıcı bulunamadı!'
END
PRINT ''

-- =============================================
-- 2. KULLANICININ ROLLERİ
-- =============================================
PRINT '2. KULLANICININ ROLLERİ:'
SELECT 
    r.[Id] as 'Rol ID',
    r.[Name] as 'Rol Adı',
    ur.[AssignedDate] as 'Atama Tarihi',
    ur.[IsActive] as 'Aktif'
FROM [BigIntSoftware].[UserRoles] ur
INNER JOIN [BigIntSoftware].[Roles] r ON ur.[RoleId] = r.[Id]
WHERE ur.[UserId] = 1
PRINT ''

-- =============================================
-- 3. MEVCUT MENÜLER
-- =============================================
PRINT '3. MEVCUT MENÜLER:'
SELECT 
    [Id],
    [Name],
    [Controller],
    [Action],
    [IsActive],
    [IsVisible],
    [SortOrder]
FROM [BigIntSoftware].[Menus]
WHERE [IsActive] = 1
ORDER BY [SortOrder]
PRINT ''

-- =============================================
-- 4. ROL MENÜ YETKİLERİ
-- =============================================
PRINT '4. ROL MENÜ YETKİLERİ:'
SELECT 
    r.[Name] as 'Rol Adı',
    m.[Name] as 'Menü Adı',
    rmp.[PermissionLevel] as 'Yetki Seviyesi',
    rmp.[IsActive] as 'Aktif'
FROM [BigIntSoftware].[RoleMenuPermissions] rmp
INNER JOIN [BigIntSoftware].[Roles] r ON rmp.[RoleId] = r.[Id]
INNER JOIN [BigIntSoftware].[Menus] m ON rmp.[MenuId] = m.[Id]
WHERE rmp.[IsActive] = 1
ORDER BY r.[Name], m.[Name]
PRINT ''

-- =============================================
-- 5. KULLANICININ ERİŞEBİLECEĞİ MENÜLER
-- =============================================
PRINT '5. KULLANICININ ERİŞEBİLECEĞİ MENÜLER:'
SELECT DISTINCT
    m.[Id] as 'Menü ID',
    m.[Name] as 'Menü Adı',
    m.[Controller] as 'Controller',
    m.[Action] as 'Action',
    r.[Name] as 'Rol Adı',
    rmp.[PermissionLevel] as 'Yetki Seviyesi'
FROM [BigIntSoftware].[UserRoles] ur
INNER JOIN [BigIntSoftware].[RoleMenuPermissions] rmp ON ur.[RoleId] = rmp.[RoleId]
INNER JOIN [BigIntSoftware].[Menus] m ON rmp.[MenuId] = m.[Id]
INNER JOIN [BigIntSoftware].[Roles] r ON ur.[RoleId] = r.[Id]
WHERE ur.[UserId] = 1 
    AND ur.[IsActive] = 1 
    AND rmp.[IsActive] = 1 
    AND m.[IsActive] = 1 
    AND m.[IsVisible] = 1
ORDER BY m.[SortOrder]
PRINT ''

-- =============================================
-- 6. ÖZEL KONTROL: USER CONTROLLER
-- =============================================
PRINT '6. ÖZEL KONTROL: USER CONTROLLER:'
SELECT 
    m.[Id] as 'Menü ID',
    m.[Name] as 'Menü Adı',
    m.[Controller] as 'Controller',
    m.[Action] as 'Action'
FROM [BigIntSoftware].[Menus] m
WHERE m.[Controller] = 'User' AND m.[Action] = 'Index' AND m.[IsActive] = 1

-- Bu menü için kullanıcının yetkisi var mı?
IF EXISTS (
    SELECT 1 
    FROM [BigIntSoftware].[UserRoles] ur
    INNER JOIN [BigIntSoftware].[RoleMenuPermissions] rmp ON ur.[RoleId] = rmp.[RoleId]
    INNER JOIN [BigIntSoftware].[Menus] m ON rmp.[MenuId] = m.[Id]
    WHERE ur.[UserId] = 1 
        AND ur.[IsActive] = 1 
        AND rmp.[IsActive] = 1 
        AND m.[Controller] = 'User' 
        AND m.[Action] = 'Index'
        AND m.[IsActive] = 1
)
BEGIN
    PRINT 'KULLANICI USER CONTROLLER''A ERİŞEBİLİR'
END
ELSE
BEGIN
    PRINT 'HATA: KULLANICI USER CONTROLLER''A ERİŞEMİYOR!'
END
PRINT ''

-- =============================================
-- 7. ÖZEL KONTROL: HOME CONTROLLER
-- =============================================
PRINT '7. ÖZEL KONTROL: HOME CONTROLLER:'
SELECT 
    m.[Id] as 'Menü ID',
    m.[Name] as 'Menü Adı',
    m.[Controller] as 'Controller',
    m.[Action] as 'Action'
FROM [BigIntSoftware].[Menus] m
WHERE m.[Controller] = 'Home' AND m.[Action] = 'Index' AND m.[IsActive] = 1

-- Bu menü için kullanıcının yetkisi var mı?
IF EXISTS (
    SELECT 1 
    FROM [BigIntSoftware].[UserRoles] ur
    INNER JOIN [BigIntSoftware].[RoleMenuPermissions] rmp ON ur.[RoleId] = rmp.[RoleId]
    INNER JOIN [BigIntSoftware].[Menus] m ON rmp.[MenuId] = m.[Id]
    WHERE ur.[UserId] = 1 
        AND ur.[IsActive] = 1 
        AND rmp.[IsActive] = 1 
        AND m.[Controller] = 'Home' 
        AND m.[Action] = 'Index'
        AND m.[IsActive] = 1
)
BEGIN
    PRINT 'KULLANICI HOME CONTROLLER''A ERİŞEBİLİR'
END
ELSE
BEGIN
    PRINT 'HATA: KULLANICI HOME CONTROLLER''A ERİŞEMİYOR!'
END
PRINT ''

PRINT '=== KONTROL TAMAMLANDI ==='
GO
