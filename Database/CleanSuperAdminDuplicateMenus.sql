-- Süper Admin rolündeki duplicate menü yetkilerini temizle
-- Önce hangi menülerin duplicate olduğunu göster
SELECT 
    rp.RoleId,
    rp.MenuId,
    m.Name as MenuName,
    COUNT(*) as DuplicateCount
FROM [BigIntSoftware].[RolePermissions] rp
INNER JOIN [BigIntSoftware].[Roles] r ON rp.RoleId = r.Id
LEFT JOIN [BigIntSoftware].[Menus] m ON rp.MenuId = m.Id
WHERE r.Name = 'Super Admin' AND rp.IsActive = 1
GROUP BY rp.RoleId, rp.MenuId, m.Name
HAVING COUNT(*) > 1
ORDER BY COUNT(*) DESC;

-- Duplicate kayıtları temizle (en eski kaydı bırak, diğerlerini sil)
WITH DuplicatePermissions AS (
    SELECT 
        rp.Id,
        ROW_NUMBER() OVER (PARTITION BY rp.RoleId, rp.MenuId ORDER BY rp.CreatedDate ASC) as RowNum
    FROM [BigIntSoftware].[RolePermissions] rp
    INNER JOIN [BigIntSoftware].[Roles] r ON rp.RoleId = r.Id
    WHERE r.Name = 'Super Admin' AND rp.IsActive = 1
)
DELETE FROM [BigIntSoftware].[RolePermissions]
WHERE Id IN (
    SELECT Id FROM DuplicatePermissions WHERE RowNum > 1
);

-- Temizlik sonrası kontrol
SELECT 
    rp.RoleId,
    rp.MenuId,
    m.Name as MenuName,
    COUNT(*) as PermissionCount
FROM [BigIntSoftware].[RolePermissions] rp
INNER JOIN [BigIntSoftware].[Roles] r ON rp.RoleId = r.Id
LEFT JOIN [BigIntSoftware].[Menus] m ON rp.MenuId = m.Id
WHERE r.Name = 'Super Admin' AND rp.IsActive = 1
GROUP BY rp.RoleId, rp.MenuId, m.Name
ORDER BY m.Name;
