-- Süper Admin rolündeki menü yetkilerini kontrol et
SELECT 
    rp.Id as RolePermissionId,
    rp.RoleId,
    r.Name as RoleName,
    rp.MenuId,
    m.Name as MenuName,
    rp.IsActive,
    rp.CreatedDate
FROM [BigIntSoftware].[RolePermissions] rp
INNER JOIN [BigIntSoftware].[Roles] r ON rp.RoleId = r.Id
LEFT JOIN [BigIntSoftware].[Menus] m ON rp.MenuId = m.Id
WHERE r.Name = 'Super Admin'
ORDER BY m.Name, rp.CreatedDate;

-- Süper Admin rolündeki aktif menü yetkilerini say
SELECT 
    m.Name as MenuName,
    COUNT(*) as PermissionCount
FROM [BigIntSoftware].[RolePermissions] rp
INNER JOIN [BigIntSoftware].[Roles] r ON rp.RoleId = r.Id
LEFT JOIN [BigIntSoftware].[Menus] m ON rp.MenuId = m.Id
WHERE r.Name = 'Super Admin' AND rp.IsActive = 1
GROUP BY m.Name, m.Id
HAVING COUNT(*) > 1
ORDER BY COUNT(*) DESC;
