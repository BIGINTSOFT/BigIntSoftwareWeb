-- BigIntSoftware Database - Örnek Veri Ekleme Scripti
-- Bu script tablolara örnek veriler ekler

USE [BigIntSoftware]
GO

-- 1. Users tablosuna örnek veriler
INSERT INTO [BigIntSoftware].[Users] ([Username], [Email], [Password], [FirstName], [LastName], [IsActive], [CreatedDate])
VALUES 
    ('admin', 'admin@bigintsoftware.com', 'admin123', 'Admin', 'User', 1, GETDATE()),
    ('manager', 'manager@bigintsoftware.com', 'manager123', 'Manager', 'User', 1, GETDATE()),
    ('user1', 'user1@bigintsoftware.com', 'user123', 'John', 'Doe', 1, GETDATE()),
    ('user2', 'user2@bigintsoftware.com', 'user123', 'Jane', 'Smith', 1, GETDATE()),
    ('testuser', 'test@bigintsoftware.com', 'test123', 'Test', 'User', 1, GETDATE())
GO

-- 2. Roles tablosuna örnek veriler
INSERT INTO [BigIntSoftware].[Roles] ([Name], [Description], [IsActive], [CreatedDate])
VALUES 
    ('Administrator', 'Sistem yöneticisi - tüm yetkilere sahip', 1, GETDATE()),
    ('Manager', 'Yönetici - kısıtlı yönetim yetkileri', 1, GETDATE()),
    ('User', 'Normal kullanıcı - temel yetkiler', 1, GETDATE()),
    ('Viewer', 'Sadece görüntüleme yetkisi', 1, GETDATE()),
    ('Editor', 'Düzenleme yetkisi olan kullanıcı', 1, GETDATE())
GO

-- 3. Menus tablosuna örnek veriler
INSERT INTO [BigIntSoftware].[Menus] ([Name], [Description], [Icon], [Url], [Controller], [Action], [ParentId], [SortOrder], [IsActive], [IsVisible], [CreatedDate])
VALUES 
    -- Ana menüler
    ('Dashboard', 'Ana sayfa', 'fas fa-home', '/Home', 'Home', 'Index', NULL, 1, 1, 1, GETDATE()),
    ('User Management', 'Kullanıcı yönetimi', 'fas fa-users', '/User', 'User', 'Index', NULL, 2, 1, 1, GETDATE()),
    ('Role Management', 'Rol yönetimi', 'fas fa-user-tag', '/Role', 'Role', 'Index', NULL, 3, 1, 1, GETDATE()),
    ('Menu Management', 'Menü yönetimi', 'fas fa-bars', '/Menu', 'Menu', 'Index', NULL, 4, 1, 1, GETDATE()),
    ('Permission Management', 'İzin yönetimi', 'fas fa-key', '/Permission', 'Permission', 'Index', NULL, 5, 1, 1, GETDATE()),
    ('Settings', 'Ayarlar', 'fas fa-cog', '/Settings', 'Settings', 'Index', NULL, 6, 1, 1, GETDATE()),
    
    -- Alt menüler (User Management altında)
    ('Create User', 'Yeni kullanıcı oluştur', 'fas fa-user-plus', '/User/Create', 'User', 'Create', 2, 1, 1, 1, GETDATE()),
    ('Edit User', 'Kullanıcı düzenle', 'fas fa-user-edit', '/User/Edit', 'User', 'Edit', 2, 2, 1, 1, GETDATE()),
    ('Delete User', 'Kullanıcı sil', 'fas fa-user-times', '/User/Delete', 'User', 'Delete', 2, 3, 1, 1, GETDATE()),
    
    -- Alt menüler (Role Management altında)
    ('Create Role', 'Yeni rol oluştur', 'fas fa-plus', '/Role/Create', 'Role', 'Create', 3, 1, 1, 1, GETDATE()),
    ('Edit Role', 'Rol düzenle', 'fas fa-edit', '/Role/Edit', 'Role', 'Edit', 3, 2, 1, 1, GETDATE()),
    ('Delete Role', 'Rol sil', 'fas fa-trash', '/Role/Delete', 'Role', 'Delete', 3, 3, 1, 1, GETDATE())
GO

-- 4. Permissions tablosuna örnek veriler
INSERT INTO [BigIntSoftware].[Permissions] ([Name], [Description], [Code], [IsActive], [CreatedDate])
VALUES 
    ('View', 'Görüntüleme yetkisi', 'VIEW', 1, GETDATE()),
    ('Create', 'Oluşturma yetkisi', 'CREATE', 1, GETDATE()),
    ('Edit', 'Düzenleme yetkisi', 'EDIT', 1, GETDATE()),
    ('Delete', 'Silme yetkisi', 'DELETE', 1, GETDATE()),
    ('Export', 'Dışa aktarma yetkisi', 'EXPORT', 1, GETDATE()),
    ('Import', 'İçe aktarma yetkisi', 'IMPORT', 1, GETDATE()),
    ('Print', 'Yazdırma yetkisi', 'PRINT', 1, GETDATE()),
    ('Approve', 'Onaylama yetkisi', 'APPROVE', 1, GETDATE())
GO

-- 5. UserRoles tablosuna örnek veriler
INSERT INTO [BigIntSoftware].[UserRoles] ([UserId], [RoleId], [AssignedDate], [IsActive], [AssignedBy], [Notes])
VALUES 
    (1, 1, GETDATE(), 1, 1, 'Admin kullanıcısına Administrator rolü atandı'),
    (2, 2, GETDATE(), 1, 1, 'Manager kullanıcısına Manager rolü atandı'),
    (3, 3, GETDATE(), 1, 2, 'User1 kullanıcısına User rolü atandı'),
    (4, 3, GETDATE(), 1, 2, 'User2 kullanıcısına User rolü atandı'),
    (5, 4, GETDATE(), 1, 1, 'Test kullanıcısına Viewer rolü atandı')
GO

-- 6. RoleMenus tablosuna örnek veriler
INSERT INTO [BigIntSoftware].[RoleMenus] ([RoleId], [MenuId], [AssignedDate], [IsActive], [AssignedBy], [Notes])
VALUES 
    -- Administrator rolü - tüm menülere erişim
    (1, 1, GETDATE(), 1, 1, 'Admin - Dashboard'),
    (1, 2, GETDATE(), 1, 1, 'Admin - User Management'),
    (1, 3, GETDATE(), 1, 1, 'Admin - Role Management'),
    (1, 4, GETDATE(), 1, 1, 'Admin - Menu Management'),
    (1, 5, GETDATE(), 1, 1, 'Admin - Permission Management'),
    (1, 6, GETDATE(), 1, 1, 'Admin - Settings'),
    (1, 7, GETDATE(), 1, 1, 'Admin - Create User'),
    (1, 8, GETDATE(), 1, 1, 'Admin - Edit User'),
    (1, 9, GETDATE(), 1, 1, 'Admin - Delete User'),
    (1, 10, GETDATE(), 1, 1, 'Admin - Create Role'),
    (1, 11, GETDATE(), 1, 1, 'Admin - Edit Role'),
    (1, 12, GETDATE(), 1, 1, 'Admin - Delete Role'),
    
    -- Manager rolü - kısıtlı erişim
    (2, 1, GETDATE(), 1, 1, 'Manager - Dashboard'),
    (2, 2, GETDATE(), 1, 1, 'Manager - User Management'),
    (2, 3, GETDATE(), 1, 1, 'Manager - Role Management'),
    (2, 7, GETDATE(), 1, 1, 'Manager - Create User'),
    (2, 8, GETDATE(), 1, 1, 'Manager - Edit User'),
    (2, 10, GETDATE(), 1, 1, 'Manager - Create Role'),
    (2, 11, GETDATE(), 1, 1, 'Manager - Edit Role'),
    
    -- User rolü - temel erişim
    (3, 1, GETDATE(), 1, 1, 'User - Dashboard'),
    (3, 2, GETDATE(), 1, 1, 'User - User Management'),
    
    -- Viewer rolü - sadece görüntüleme
    (4, 1, GETDATE(), 1, 1, 'Viewer - Dashboard'),
    (4, 2, GETDATE(), 1, 1, 'Viewer - User Management'),
    (4, 3, GETDATE(), 1, 1, 'Viewer - Role Management')
GO

-- 7. RoleMenuPermissions tablosuna örnek veriler
INSERT INTO [BigIntSoftware].[RoleMenuPermissions] ([RoleId], [MenuId], [PermissionId], [PermissionLevel], [AssignedDate], [IsActive], [AssignedBy], [Notes])
VALUES 
    -- Administrator rolü - tüm menülerde tüm izinler
    (1, 1, 1, 'VIEW', GETDATE(), 1, 1, 'Admin - Dashboard View'),
    (1, 1, 2, 'CREATE', GETDATE(), 1, 1, 'Admin - Dashboard Create'),
    (1, 1, 3, 'EDIT', GETDATE(), 1, 1, 'Admin - Dashboard Edit'),
    (1, 1, 4, 'DELETE', GETDATE(), 1, 1, 'Admin - Dashboard Delete'),
    
    (1, 2, 1, 'VIEW', GETDATE(), 1, 1, 'Admin - User Management View'),
    (1, 2, 2, 'CREATE', GETDATE(), 1, 1, 'Admin - User Management Create'),
    (1, 2, 3, 'EDIT', GETDATE(), 1, 1, 'Admin - User Management Edit'),
    (1, 2, 4, 'DELETE', GETDATE(), 1, 1, 'Admin - User Management Delete'),
    
    (1, 3, 1, 'VIEW', GETDATE(), 1, 1, 'Admin - Role Management View'),
    (1, 3, 2, 'CREATE', GETDATE(), 1, 1, 'Admin - Role Management Create'),
    (1, 3, 3, 'EDIT', GETDATE(), 1, 1, 'Admin - Role Management Edit'),
    (1, 3, 4, 'DELETE', GETDATE(), 1, 1, 'Admin - Role Management Delete'),
    
    -- Manager rolü - kısıtlı izinler
    (2, 1, 1, 'VIEW', GETDATE(), 1, 1, 'Manager - Dashboard View'),
    (2, 2, 1, 'VIEW', GETDATE(), 1, 1, 'Manager - User Management View'),
    (2, 2, 2, 'CREATE', GETDATE(), 1, 1, 'Manager - User Management Create'),
    (2, 2, 3, 'EDIT', GETDATE(), 1, 1, 'Manager - User Management Edit'),
    (2, 3, 1, 'VIEW', GETDATE(), 1, 1, 'Manager - Role Management View'),
    (2, 3, 2, 'CREATE', GETDATE(), 1, 1, 'Manager - Role Management Create'),
    (2, 3, 3, 'EDIT', GETDATE(), 1, 1, 'Manager - Role Management Edit'),
    
    -- User rolü - sadece görüntüleme
    (3, 1, 1, 'VIEW', GETDATE(), 1, 1, 'User - Dashboard View'),
    (3, 2, 1, 'VIEW', GETDATE(), 1, 1, 'User - User Management View'),
    
    -- Viewer rolü - sadece görüntüleme
    (4, 1, 1, 'VIEW', GETDATE(), 1, 1, 'Viewer - Dashboard View'),
    (4, 2, 1, 'VIEW', GETDATE(), 1, 1, 'Viewer - User Management View'),
    (4, 3, 1, 'VIEW', GETDATE(), 1, 1, 'Viewer - Role Management View')
GO

-- 8. UserMenus tablosuna örnek veriler (kullanıcılara özel menü atamaları)
INSERT INTO [BigIntSoftware].[UserMenus] ([UserId], [MenuId], [AssignedDate], [IsActive], [AssignedBy], [Notes])
VALUES 
    (3, 4, GETDATE(), 1, 1, 'User1 - Menu Management özel erişim'),
    (4, 5, GETDATE(), 1, 1, 'User2 - Permission Management özel erişim'),
    (5, 6, GETDATE(), 1, 1, 'TestUser - Settings özel erişim')
GO

-- 9. UserMenuPermissions tablosuna örnek veriler (kullanıcılara özel izin atamaları)
INSERT INTO [BigIntSoftware].[UserMenuPermissions] ([UserId], [MenuId], [PermissionId], [PermissionLevel], [AssignedDate], [IsActive], [AssignedBy], [Notes])
VALUES 
    (3, 4, 1, 'VIEW', GETDATE(), 1, 1, 'User1 - Menu Management View özel izin'),
    (3, 4, 2, 'CREATE', GETDATE(), 1, 1, 'User1 - Menu Management Create özel izin'),
    (4, 5, 1, 'VIEW', GETDATE(), 1, 1, 'User2 - Permission Management View özel izin'),
    (4, 5, 3, 'EDIT', GETDATE(), 1, 1, 'User2 - Permission Management Edit özel izin'),
    (5, 6, 1, 'VIEW', GETDATE(), 1, 1, 'TestUser - Settings View özel izin')
GO

PRINT 'Örnek veriler başarıyla eklendi!'
PRINT 'Eklenen veriler:'
PRINT '- 5 kullanıcı'
PRINT '- 5 rol'
PRINT '- 12 menü (ana ve alt menüler)'
PRINT '- 8 izin'
PRINT '- 5 kullanıcı-rol ataması'
PRINT '- 20 rol-menü ataması'
PRINT '- 25 rol-menü-izin ataması'
PRINT '- 3 kullanıcı-menü ataması'
PRINT '- 5 kullanıcı-menü-izin ataması'
