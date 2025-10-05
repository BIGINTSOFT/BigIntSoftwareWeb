-- Super Admin rolüne tüm menü yetkilerini ata
-- Bu script Super Admin rolünün tüm menülere erişim yetkisi olmasını sağlar

-- 1. Önce mevcut Super Admin menü yetkilerini temizle (isteğe bağlı)
-- DELETE FROM BigIntSoftware.RolePermissions 
-- WHERE RoleId = (SELECT Id FROM BigIntSoftware.Roles WHERE Name = 'Super Admin');

-- 2. Super Admin rolünün ID'sini al
DECLARE @SuperAdminRoleId INT;
SELECT @SuperAdminRoleId = Id FROM BigIntSoftware.Roles WHERE Name = 'Super Admin';

-- 3. Tüm aktif ve görünür menüleri Super Admin rolüne ata
INSERT INTO BigIntSoftware.RolePermissions (RoleId, MenuId, PermissionId, IsActive, CreatedDate, CreatedBy)
SELECT 
    @SuperAdminRoleId as RoleId,
    m.Id as MenuId,
    NULL as PermissionId, -- Menü yetkisi için PermissionId NULL
    1 as IsActive,
    GETDATE() as CreatedDate,
    'System' as CreatedBy
FROM BigIntSoftware.Menus m
WHERE m.IsActive = 1 AND m.IsVisible = 1
AND NOT EXISTS (
    -- Zaten atanmış olan menüleri atlama
    SELECT 1 FROM BigIntSoftware.RolePermissions rp 
    WHERE rp.RoleId = @SuperAdminRoleId 
    AND rp.MenuId = m.Id 
    AND rp.IsActive = 1
);

-- 4. Sonucu kontrol et
SELECT 'Super Admin Role Menu Permissions After Update:' as Info;
SELECT r.Name as RoleName, m.Name as MenuName, m.Controller, m.Action, rp.IsActive
FROM BigIntSoftware.Roles r
INNER JOIN BigIntSoftware.RolePermissions rp ON r.Id = rp.RoleId
INNER JOIN BigIntSoftware.Menus m ON rp.MenuId = m.Id
WHERE r.Name = 'Super Admin' AND rp.IsActive = 1
ORDER BY m.SortOrder;

-- 5. Admin kullanıcısının menü yetkilerini kontrol et
SELECT 'Admin User Menu Permissions (via Super Admin role):' as Info;
SELECT u.Username, r.Name as RoleName, m.Name as MenuName, m.Controller, m.Action
FROM BigIntSoftware.Users u
INNER JOIN BigIntSoftware.UserRoles ur ON u.Id = ur.UserId
INNER JOIN BigIntSoftware.Roles r ON ur.RoleId = r.Id
INNER JOIN BigIntSoftware.RolePermissions rp ON r.Id = rp.RoleId
INNER JOIN BigIntSoftware.Menus m ON rp.MenuId = m.Id
WHERE u.Username = 'admin' 
AND ur.IsActive = 1 
AND rp.IsActive = 1 
AND m.IsActive = 1 
AND m.IsVisible = 1
ORDER BY m.SortOrder;
