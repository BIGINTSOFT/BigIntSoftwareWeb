-- Kullanıcı menü yetkilerini kontrol et
-- Admin kullanıcısının Super Admin rolünden dolayı sahip olduğu menüleri göster

-- 1. Admin kullanıcısının ID'sini bul
SELECT 'Admin User Info:' as Info;
SELECT Id, Username, FirstName, LastName, IsActive 
FROM BigIntSoftware.Users 
WHERE Username = 'admin';

-- 2. Admin kullanıcısının rollerini göster
SELECT 'Admin User Roles:' as Info;
SELECT u.Username, r.Name as RoleName, ur.IsActive
FROM BigIntSoftware.Users u
INNER JOIN BigIntSoftware.UserRoles ur ON u.Id = ur.UserId
INNER JOIN BigIntSoftware.Roles r ON ur.RoleId = r.Id
WHERE u.Username = 'admin';

-- 3. Super Admin rolünün menü yetkilerini göster
SELECT 'Super Admin Role Menu Permissions:' as Info;
SELECT r.Name as RoleName, m.Name as MenuName, m.Controller, m.Action, rp.IsActive
FROM BigIntSoftware.Roles r
INNER JOIN BigIntSoftware.RolePermissions rp ON r.Id = rp.RoleId
INNER JOIN BigIntSoftware.Menus m ON rp.MenuId = m.Id
WHERE r.Name = 'Super Admin' AND rp.IsActive = 1
ORDER BY m.SortOrder;

-- 4. Admin kullanıcısının direkt menü yetkilerini göster
SELECT 'Admin User Direct Menu Permissions:' as Info;
SELECT u.Username, m.Name as MenuName, m.Controller, m.Action, up.IsActive
FROM BigIntSoftware.Users u
INNER JOIN BigIntSoftware.UserPermissions up ON u.Id = up.UserId
INNER JOIN BigIntSoftware.Menus m ON up.MenuId = m.Id
WHERE u.Username = 'admin' AND up.IsActive = 1
ORDER BY m.SortOrder;

-- 5. Tüm menüleri göster (karşılaştırma için)
SELECT 'All Active Menus:' as Info;
SELECT Id, Name, Controller, Action, IsActive, IsVisible, SortOrder
FROM BigIntSoftware.Menus
WHERE IsActive = 1 AND IsVisible = 1
ORDER BY SortOrder;
