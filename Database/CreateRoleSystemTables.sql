-- BigInt Software ERP - Role and Permission System Tables
-- Created: 2024

USE [bigintsoft]
GO

-- Roles Table
CREATE TABLE [BigIntSoftware].[Roles] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(50) NOT NULL,
    [Description] nvarchar(200) NULL,
    [IsActive] bit NOT NULL DEFAULT 1,
    [CreatedDate] datetime2 NOT NULL DEFAULT GETDATE(),
    [UpdatedDate] datetime2 NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_Roles_Name] UNIQUE ([Name])
)
GO

-- Menus Table
CREATE TABLE [BigIntSoftware].[Menus] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [Description] nvarchar(200) NULL,
    [Icon] nvarchar(50) NULL,
    [Url] nvarchar(200) NULL,
    [Controller] nvarchar(100) NULL,
    [Action] nvarchar(100) NULL,
    [ParentId] int NULL,
    [SortOrder] int NOT NULL DEFAULT 0,
    [IsActive] bit NOT NULL DEFAULT 1,
    [IsVisible] bit NOT NULL DEFAULT 1,
    [CreatedDate] datetime2 NOT NULL DEFAULT GETDATE(),
    [UpdatedDate] datetime2 NULL,
    CONSTRAINT [PK_Menus] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Menus_Parent] FOREIGN KEY ([ParentId]) REFERENCES [BigIntSoftware].[Menus]([Id])
)
GO

-- Permissions Table
CREATE TABLE [BigIntSoftware].[Permissions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [Description] nvarchar(200) NULL,
    [Code] nvarchar(100) NULL,
    [IsActive] bit NOT NULL DEFAULT 1,
    [CreatedDate] datetime2 NOT NULL DEFAULT GETDATE(),
    [UpdatedDate] datetime2 NULL,
    CONSTRAINT [PK_Permissions] PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_Permissions_Code] UNIQUE ([Code])
)
GO

-- UserRoles Table
CREATE TABLE [BigIntSoftware].[UserRoles] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserId] int NOT NULL,
    [RoleId] int NOT NULL,
    [AssignedDate] datetime2 NOT NULL DEFAULT GETDATE(),
    [ExpiryDate] datetime2 NULL,
    [IsActive] bit NOT NULL DEFAULT 1,
    CONSTRAINT [PK_UserRoles] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UserRoles_Users] FOREIGN KEY ([UserId]) REFERENCES [BigIntSoftware].[Users]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserRoles_Roles] FOREIGN KEY ([RoleId]) REFERENCES [BigIntSoftware].[Roles]([Id]) ON DELETE CASCADE,
    CONSTRAINT [UQ_UserRoles_User_Role] UNIQUE ([UserId], [RoleId])
)
GO

-- RolePermissions Table
CREATE TABLE [BigIntSoftware].[RolePermissions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [RoleId] int NOT NULL,
    [PermissionId] int NOT NULL,
    [MenuId] int NULL,
    [AssignedDate] datetime2 NOT NULL DEFAULT GETDATE(),
    [IsActive] bit NOT NULL DEFAULT 1,
    CONSTRAINT [PK_RolePermissions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RolePermissions_Roles] FOREIGN KEY ([RoleId]) REFERENCES [BigIntSoftware].[Roles]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_RolePermissions_Permissions] FOREIGN KEY ([PermissionId]) REFERENCES [BigIntSoftware].[Permissions]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_RolePermissions_Menus] FOREIGN KEY ([MenuId]) REFERENCES [BigIntSoftware].[Menus]([Id]),
    CONSTRAINT [UQ_RolePermissions_Role_Permission_Menu] UNIQUE ([RoleId], [PermissionId], [MenuId])
)
GO

-- UserPermissions Table
CREATE TABLE [BigIntSoftware].[UserPermissions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserId] int NOT NULL,
    [PermissionId] int NOT NULL,
    [MenuId] int NULL,
    [AssignedDate] datetime2 NOT NULL DEFAULT GETDATE(),
    [ExpiryDate] datetime2 NULL,
    [IsActive] bit NOT NULL DEFAULT 1,
    CONSTRAINT [PK_UserPermissions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UserPermissions_Users] FOREIGN KEY ([UserId]) REFERENCES [BigIntSoftware].[Users]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserPermissions_Permissions] FOREIGN KEY ([PermissionId]) REFERENCES [BigIntSoftware].[Permissions]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserPermissions_Menus] FOREIGN KEY ([MenuId]) REFERENCES [BigIntSoftware].[Menus]([Id]),
    CONSTRAINT [UQ_UserPermissions_User_Permission_Menu] UNIQUE ([UserId], [PermissionId], [MenuId])
)
GO

-- Insert Default Roles
INSERT INTO [BigIntSoftware].[Roles] ([Name], [Description]) VALUES
('SuperAdmin', 'Sistem Yöneticisi - Tüm yetkilere sahip'),
('Admin', 'Yönetici - Çoğu yetkiye sahip'),
('Manager', 'Müdür - Sınırlı yönetim yetkisi'),
('User', 'Kullanıcı - Temel yetkiler'),
('Guest', 'Misafir - Sadece görüntüleme yetkisi')
GO

-- Insert Default Permissions
INSERT INTO [BigIntSoftware].[Permissions] ([Name], [Description], [Code]) VALUES
('View', 'Görüntüleme Yetkisi', 'VIEW'),
('Create', 'Oluşturma Yetkisi', 'CREATE'),
('Edit', 'Düzenleme Yetkisi', 'EDIT'),
('Delete', 'Silme Yetkisi', 'DELETE'),
('Export', 'Dışa Aktarma Yetkisi', 'EXPORT'),
('Import', 'İçe Aktarma Yetkisi', 'IMPORT'),
('Print', 'Yazdırma Yetkisi', 'PRINT'),
('Manage', 'Yönetim Yetkisi', 'MANAGE')
GO

-- Insert Default Menus
INSERT INTO [BigIntSoftware].[Menus] ([Name], [Description], [Icon], [Controller], [Action], [ParentId], [SortOrder]) VALUES
('Dashboard', 'Ana Sayfa', 'bi-speedometer2', 'Home', 'Index', NULL, 1),
('Kullanıcı Yönetimi', 'Kullanıcı işlemleri', 'bi-people', 'User', 'Index', NULL, 2),
('Envanter Yönetimi', 'Envanter işlemleri', 'bi-boxes', NULL, NULL, NULL, 3),
('Satış Yönetimi', 'Satış işlemleri', 'bi-cart3', NULL, NULL, NULL, 4),
('Raporlar', 'Rapor işlemleri', 'bi-pie-chart', 'Reports', 'Index', NULL, 5),
('Ayarlar', 'Sistem ayarları', 'bi-gear', 'Settings', 'Index', NULL, 6)
GO

-- Insert Sub Menus for Envanter Yönetimi
INSERT INTO [BigIntSoftware].[Menus] ([Name], [Description], [Icon], [Controller], [Action], [ParentId], [SortOrder]) VALUES
('Ürün Listesi', 'Ürün listesi görüntüleme', 'bi-list', 'Product', 'Index', 3, 1),
('Yeni Ürün', 'Yeni ürün ekleme', 'bi-plus', 'Product', 'Create', 3, 2),
('Stok Raporları', 'Stok raporları', 'bi-bar-chart', 'Product', 'StockReport', 3, 3)
GO

-- Insert Sub Menus for Satış Yönetimi
INSERT INTO [BigIntSoftware].[Menus] ([Name], [Description], [Icon], [Controller], [Action], [ParentId], [SortOrder]) VALUES
('Siparişler', 'Sipariş listesi', 'bi-list', 'Order', 'Index', 4, 1),
('Yeni Sipariş', 'Yeni sipariş oluşturma', 'bi-plus', 'Order', 'Create', 4, 2),
('Satış Raporları', 'Satış raporları', 'bi-graph-up', 'Order', 'SalesReport', 4, 3)
GO

-- Insert Sub Menus for Ayarlar
INSERT INTO [BigIntSoftware].[Menus] ([Name], [Description], [Icon], [Controller], [Action], [ParentId], [SortOrder]) VALUES
('Rol Yönetimi', 'Rol işlemleri', 'bi-shield-check', 'Role', 'Index', 6, 1),
('Menü Yönetimi', 'Menü işlemleri', 'bi-list-ul', 'Menu', 'Index', 6, 2),
('Yetki Yönetimi', 'Yetki işlemleri', 'bi-key', 'Permission', 'Index', 6, 3)
GO

-- Assign SuperAdmin role to admin user
INSERT INTO [BigIntSoftware].[UserRoles] ([UserId], [RoleId]) 
SELECT u.Id, r.Id 
FROM [BigIntSoftware].[Users] u, [BigIntSoftware].[Roles] r 
WHERE u.Username = 'admin' AND r.Name = 'SuperAdmin'
GO

-- Assign all permissions to SuperAdmin role
INSERT INTO [BigIntSoftware].[RolePermissions] ([RoleId], [PermissionId], [MenuId])
SELECT r.Id, p.Id, m.Id
FROM [BigIntSoftware].[Roles] r
CROSS JOIN [BigIntSoftware].[Permissions] p
CROSS JOIN [BigIntSoftware].[Menus] m
WHERE r.Name = 'SuperAdmin'
GO

PRINT 'Role and Permission System tables created successfully!'
