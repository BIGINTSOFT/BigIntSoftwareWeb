-- ADIM 1: Önce hangi tablolarda referans var kontrol et
SELECT 
    t.name AS TableName,
    c.name AS ColumnName,
    fk.name AS ForeignKeyName
FROM sys.foreign_keys fk
INNER JOIN sys.tables t ON fk.parent_object_id = t.object_id
INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
INNER JOIN sys.columns c ON fkc.parent_column_id = c.column_id AND fkc.parent_object_id = c.object_id
WHERE fk.referenced_object_id = (SELECT object_id FROM sys.tables WHERE name = 'Menus' AND schema_id = SCHEMA_ID('BigIntSoftware'))

-- ADIM 2: RolePermissions tablosundaki referansları kontrol et
SELECT COUNT(*) as RolePermissionCount
FROM [BigIntSoftware].[RolePermissions] 
WHERE MenuId IN (3, 4, 5)

-- ADIM 3: UserPermissions tablosundaki referansları kontrol et
SELECT COUNT(*) as UserPermissionCount
FROM [BigIntSoftware].[UserPermissions] 
WHERE MenuId IN (3, 4, 5)

-- ADIM 4: Eğer referanslar varsa, önce onları temizle
-- (Yukarıdaki sorgular 0 döndürürse bu adımları atlayın)

-- DELETE FROM [BigIntSoftware].[RolePermissions] WHERE MenuId IN (3, 4, 5)
-- DELETE FROM [BigIntSoftware].[UserPermissions] WHERE MenuId IN (3, 4, 5)

-- ADIM 5: Menüleri güncelle (silme yerine)
UPDATE [BigIntSoftware].[Menus] 
SET IsActive = 0, IsVisible = 0, Name = Name + ' (KALDIRILDI)'
WHERE Id IN (3, 4, 5)

-- ADIM 6: Kullanıcı Yönetimi'nin ParentId'sini Ayarlar'a (ID: 9) çevir
UPDATE [BigIntSoftware].[Menus] 
SET ParentId = 9, SortOrder = 1
WHERE Id = 2 -- Kullanıcı Yönetimi

-- ADIM 7: Rol, Menü, Yetki Yönetimi'nin ParentId'sini Kullanıcı Yönetimi'ne (ID: 2) çevir
UPDATE [BigIntSoftware].[Menus] 
SET ParentId = 2, SortOrder = 1
WHERE Id = 16 -- Rol Yönetimi

UPDATE [BigIntSoftware].[Menus] 
SET ParentId = 2, SortOrder = 2
WHERE Id = 17 -- Menü Yönetimi

UPDATE [BigIntSoftware].[Menus] 
SET ParentId = 2, SortOrder = 3
WHERE Id = 18 -- Yetki Yönetimi

-- ADIM 8: Ayarlar menüsünün SortOrder'ını düzenle
UPDATE [BigIntSoftware].[Menus] 
SET SortOrder = 6
WHERE Id = 9 -- Ayarlar

-- ADIM 9: Sonuç kontrolü
SELECT Id, Name, ParentId, SortOrder, IsActive, IsVisible
FROM [BigIntSoftware].[Menus] 
WHERE IsActive = 1
ORDER BY ParentId, SortOrder
