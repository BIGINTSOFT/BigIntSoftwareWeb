-- Soft Delete yaklaşımı - menüleri silmek yerine pasif yap

-- Duplicate menüleri pasif yap (ParentId olmayanları)
UPDATE [BigIntSoftware].[Menus] 
SET IsActive = 0, IsVisible = 0
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
WHERE IsActive = 1
ORDER BY ParentId, SortOrder
