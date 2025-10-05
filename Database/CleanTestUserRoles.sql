-- Test kullanıcısının rol atamalarını temizlemek için SQL scripti

-- 1. Test kullanıcısının tüm rol atamalarını göster (temizlemeden önce)
SELECT 
    ur.Id,
    ur.UserId,
    u.Username,
    ur.RoleId,
    r.Name as RoleName,
    ur.AssignedDate,
    ur.IsActive
FROM [BigIntSoftware].[UserRoles] ur
INNER JOIN [BigIntSoftware].[Users] u ON ur.UserId = u.Id
INNER JOIN [BigIntSoftware].[Roles] r ON ur.RoleId = r.Id
WHERE u.Username = 'test' OR u.Username LIKE '%test%';

-- 2. Test kullanıcısının tüm rol atamalarını sil (dikkatli ol!)
-- Bu komutu çalıştırmadan önce yukarıdaki sorguyu çalıştırın ve sonuçları kontrol edin

-- Test kullanıcısının ID'sini bul
DECLARE @TestUserId INT;
SELECT @TestUserId = Id FROM [BigIntSoftware].[Users] WHERE Username = 'test';

-- Test kullanıcısının tüm rol atamalarını sil
DELETE FROM [BigIntSoftware].[UserRoles] 
WHERE UserId = @TestUserId;

-- 3. Temizleme sonrası kontrol
SELECT 
    ur.Id,
    ur.UserId,
    u.Username,
    ur.RoleId,
    r.Name as RoleName,
    ur.AssignedDate,
    ur.IsActive
FROM [BigIntSoftware].[UserRoles] ur
INNER JOIN [BigIntSoftware].[Users] u ON ur.UserId = u.Id
INNER JOIN [BigIntSoftware].[Roles] r ON ur.RoleId = r.Id
WHERE u.Username = 'test' OR u.Username LIKE '%test%';

-- 4. Test kullanıcısını Admin rolüne manuel olarak ata (test için)
-- Bu komutu çalıştırmadan önce yukarıdaki temizleme işlemini yapın

DECLARE @TestUserId2 INT;
DECLARE @AdminRoleId INT;

SELECT @TestUserId2 = Id FROM [BigIntSoftware].[Users] WHERE Username = 'test';
SELECT @AdminRoleId = Id FROM [BigIntSoftware].[Roles] WHERE Name = 'Admin';

-- Test kullanıcısını Admin rolüne ata
INSERT INTO [BigIntSoftware].[UserRoles] (UserId, RoleId, AssignedDate, IsActive)
VALUES (@TestUserId2, @AdminRoleId, GETDATE(), 1);

-- 5. Atama sonrası kontrol
SELECT 
    ur.Id,
    ur.UserId,
    u.Username,
    ur.RoleId,
    r.Name as RoleName,
    ur.AssignedDate,
    ur.IsActive
FROM [BigIntSoftware].[UserRoles] ur
INNER JOIN [BigIntSoftware].[Users] u ON ur.UserId = u.Id
INNER JOIN [BigIntSoftware].[Roles] r ON ur.RoleId = r.Id
WHERE u.Username = 'test';
