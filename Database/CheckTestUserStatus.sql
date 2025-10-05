-- Test kullanıcısının durumunu kontrol etmek için SQL sorguları

-- 1. Tüm kullanıcıları listele
SELECT 
    Id,
    Username,
    FirstName,
    LastName,
    Email,
    IsActive,
    CreatedDate
FROM [BigIntSoftware].[Users]
ORDER BY Id;

-- 2. Test kullanıcısının detaylarını göster
SELECT 
    Id,
    Username,
    FirstName,
    LastName,
    Email,
    IsActive,
    CreatedDate,
    LastLoginDate
FROM [BigIntSoftware].[Users]
WHERE Username = 'test' OR Username LIKE '%test%';

-- 3. Tüm rolleri listele
SELECT 
    Id,
    Name,
    Description,
    IsActive,
    CreatedDate
FROM [BigIntSoftware].[Roles]
ORDER BY Id;

-- 4. Test kullanıcısının mevcut rol atamalarını kontrol et
SELECT 
    ur.Id,
    ur.UserId,
    u.Username,
    ur.RoleId,
    r.Name as RoleName,
    ur.AssignedDate,
    ur.IsActive,
    ur.ExpiryDate
FROM [BigIntSoftware].[UserRoles] ur
INNER JOIN [BigIntSoftware].[Users] u ON ur.UserId = u.Id
INNER JOIN [BigIntSoftware].[Roles] r ON ur.RoleId = r.Id
WHERE u.Username = 'test' OR u.Username LIKE '%test%';

-- 5. Test kullanıcısının tüm rol atamalarını (aktif ve pasif) göster
SELECT 
    ur.Id,
    ur.UserId,
    u.Username,
    ur.RoleId,
    r.Name as RoleName,
    ur.AssignedDate,
    ur.IsActive,
    ur.ExpiryDate
FROM [BigIntSoftware].[UserRoles] ur
INNER JOIN [BigIntSoftware].[Users] u ON ur.UserId = u.Id
INNER JOIN [BigIntSoftware].[Roles] r ON ur.RoleId = r.Id
WHERE u.Username = 'test' OR u.Username LIKE '%test%'
ORDER BY ur.IsActive DESC, ur.AssignedDate DESC;

-- 6. Belirli bir role (örneğin Admin) atanmış tüm kullanıcıları göster
SELECT 
    ur.Id,
    ur.UserId,
    u.Username,
    u.FirstName,
    u.LastName,
    ur.RoleId,
    r.Name as RoleName,
    ur.AssignedDate,
    ur.IsActive
FROM [BigIntSoftware].[UserRoles] ur
INNER JOIN [BigIntSoftware].[Users] u ON ur.UserId = u.Id
INNER JOIN [BigIntSoftware].[Roles] r ON ur.RoleId = r.Id
WHERE r.Name = 'Admin' AND ur.IsActive = 1;

-- 7. Test kullanıcısının ID'sini bul
SELECT Id, Username FROM [BigIntSoftware].[Users] WHERE Username = 'test';

-- 8. Admin rolünün ID'sini bul
SELECT Id, Name FROM [BigIntSoftware].[Roles] WHERE Name = 'Admin';

-- 9. Test kullanıcısı ve Admin rolü arasında aktif atama var mı kontrol et
SELECT 
    ur.Id,
    ur.UserId,
    ur.RoleId,
    ur.IsActive,
    ur.AssignedDate
FROM [BigIntSoftware].[UserRoles] ur
INNER JOIN [BigIntSoftware].[Users] u ON ur.UserId = u.Id
INNER JOIN [BigIntSoftware].[Roles] r ON ur.RoleId = r.Id
WHERE u.Username = 'test' AND r.Name = 'Admin' AND ur.IsActive = 1;

-- 10. Test kullanıcısı ve Admin rolü arasında herhangi bir atama var mı (aktif/pasif) kontrol et
SELECT 
    ur.Id,
    ur.UserId,
    ur.RoleId,
    ur.IsActive,
    ur.AssignedDate
FROM [BigIntSoftware].[UserRoles] ur
INNER JOIN [BigIntSoftware].[Users] u ON ur.UserId = u.Id
INNER JOIN [BigIntSoftware].[Roles] r ON ur.RoleId = r.Id
WHERE u.Username = 'test' AND r.Name = 'Admin';
