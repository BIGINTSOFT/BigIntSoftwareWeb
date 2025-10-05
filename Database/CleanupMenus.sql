-- Önce RolePermissions ve UserPermissions tablolarından referansları temizle
DELETE FROM [BigIntSoftware].[RolePermissions] 
WHERE MenuId IN (3, 4, 5)

DELETE FROM [BigIntSoftware].[UserPermissions] 
WHERE MenuId IN (3, 4, 5)

-- Duplicate menüleri sil (ParentId olmayanları)
DELETE FROM [BigIntSoftware].[Menus] 
WHERE Id IN (3, 4, 5) -- Rol Yönetimi, Menü Yönetimi, Yetki Yönetimi (ParentId olmayanlar)

-- Kullanıcı Yönetimi'nin ParentId'sini Ayarlar'a (ID: 9) çevir
UPDATE [BigIntSoftware].[Menus] 
SET ParentId = 9, SortOrder = 1
WHERE Id = 2 -- Kullanıcı Yönetimi

-- Rol, Menü, Yetki Yönetimi'nin ParentId'sini Kullanıcı Yönetimi'ne (ID: 2) çevir
UPDATE [BigIntSoftware].[Menus] 
SET ParentId = 2, SortOrder = 1
WHERE Id = 16 -- Rol Yönetimi

UPDATE [BigIntSoftware].[Menus] 
SET ParentId = 2, SortOrder = 2
WHERE Id = 17 -- Menü Yönetimi

UPDATE [BigIntSoftware].[Menus] 
SET ParentId = 2, SortOrder = 3
WHERE Id = 18 -- Yetki Yönetimi

-- Ayarlar menüsünün SortOrder'ını düzenle
UPDATE [BigIntSoftware].[Menus] 
SET SortOrder = 6
WHERE Id = 9 -- Ayarlar

-- Sonuç kontrolü
SELECT Id, Name, ParentId, SortOrder, IsActive, IsVisible
FROM [BigIntSoftware].[Menus] 
ORDER BY ParentId, SortOrder
