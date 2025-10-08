-- BigIntSoftware Database - Yeni Tablo Yapısı
-- Bu script mevcut tabloları truncate eder ve yeni yapıyı oluşturur

USE [BigIntSoftware]
GO

-- Mevcut tabloları truncate et
TRUNCATE TABLE [BigIntSoftware].[UserExtraPermissions]
TRUNCATE TABLE [BigIntSoftware].[RoleMenuPermissions]
TRUNCATE TABLE [BigIntSoftware].[UserRoles]
TRUNCATE TABLE [BigIntSoftware].[Permissions]
TRUNCATE TABLE [BigIntSoftware].[Menus]
TRUNCATE TABLE [BigIntSoftware].[Roles]
TRUNCATE TABLE [BigIntSoftware].[Users]
GO

-- Yeni tabloları oluştur

-- 1. UserMenus Tablosu (User-Menu ilişkisi)
CREATE TABLE [BigIntSoftware].[UserMenus] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserId] int NOT NULL,
    [MenuId] int NOT NULL,
    [AssignedDate] datetime2(7) NOT NULL DEFAULT GETDATE(),
    [ExpiryDate] datetime2(7) NULL,
    [IsActive] bit NOT NULL DEFAULT 1,
    [AssignedBy] int NULL,
    [Notes] nvarchar(500) NULL,
    CONSTRAINT [PK_UserMenus] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UserMenus_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [BigIntSoftware].[Users] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserMenus_Menus_MenuId] FOREIGN KEY ([MenuId]) REFERENCES [BigIntSoftware].[Menus] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserMenus_Users_AssignedBy] FOREIGN KEY ([AssignedBy]) REFERENCES [BigIntSoftware].[Users] ([Id]) ON DELETE NO ACTION
)
GO

-- 2. UserMenuPermissions Tablosu (User-Menu-Permission ilişkisi)
CREATE TABLE [BigIntSoftware].[UserMenuPermissions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserId] int NOT NULL,
    [MenuId] int NOT NULL,
    [PermissionId] int NOT NULL,
    [PermissionLevel] nvarchar(20) NOT NULL, -- VIEW, CREATE, EDIT, DELETE
    [AssignedDate] datetime2(7) NOT NULL DEFAULT GETDATE(),
    [ExpiryDate] datetime2(7) NULL,
    [IsActive] bit NOT NULL DEFAULT 1,
    [AssignedBy] int NULL,
    [Notes] nvarchar(500) NULL,
    CONSTRAINT [PK_UserMenuPermissions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UserMenuPermissions_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [BigIntSoftware].[Users] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserMenuPermissions_Menus_MenuId] FOREIGN KEY ([MenuId]) REFERENCES [BigIntSoftware].[Menus] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserMenuPermissions_Permissions_PermissionId] FOREIGN KEY ([PermissionId]) REFERENCES [BigIntSoftware].[Permissions] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserMenuPermissions_Users_AssignedBy] FOREIGN KEY ([AssignedBy]) REFERENCES [BigIntSoftware].[Users] ([Id]) ON DELETE NO ACTION
)
GO

-- 3. RoleMenuPermissions Tablosu (Role-Menu-Permission ilişkisi)
CREATE TABLE [BigIntSoftware].[RoleMenuPermissions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [RoleId] int NOT NULL,
    [MenuId] int NOT NULL,
    [PermissionId] int NOT NULL,
    [PermissionLevel] nvarchar(20) NOT NULL, -- VIEW, CREATE, EDIT, DELETE
    [AssignedDate] datetime2(7) NOT NULL DEFAULT GETDATE(),
    [IsActive] bit NOT NULL DEFAULT 1,
    [AssignedBy] int NULL,
    [Notes] nvarchar(500) NULL,
    CONSTRAINT [PK_RoleMenuPermissions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RoleMenuPermissions_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [BigIntSoftware].[Roles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_RoleMenuPermissions_Menus_MenuId] FOREIGN KEY ([MenuId]) REFERENCES [BigIntSoftware].[Menus] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_RoleMenuPermissions_Permissions_PermissionId] FOREIGN KEY ([PermissionId]) REFERENCES [BigIntSoftware].[Permissions] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_RoleMenuPermissions_Users_AssignedBy] FOREIGN KEY ([AssignedBy]) REFERENCES [BigIntSoftware].[Users] ([Id]) ON DELETE NO ACTION
)
GO

-- 4. UserRoles Tablosu (User-Role ilişkisi)
CREATE TABLE [BigIntSoftware].[UserRoles] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserId] int NOT NULL,
    [RoleId] int NOT NULL,
    [AssignedDate] datetime2(7) NOT NULL DEFAULT GETDATE(),
    [ExpiryDate] datetime2(7) NULL,
    [IsActive] bit NOT NULL DEFAULT 1,
    [AssignedBy] int NULL,
    [Notes] nvarchar(500) NULL,
    CONSTRAINT [PK_UserRoles] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UserRoles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [BigIntSoftware].[Users] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserRoles_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [BigIntSoftware].[Roles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserRoles_Users_AssignedBy] FOREIGN KEY ([AssignedBy]) REFERENCES [BigIntSoftware].[Users] ([Id]) ON DELETE NO ACTION
)
GO

-- 5. RoleMenus Tablosu (Role-Menu ilişkisi)
CREATE TABLE [BigIntSoftware].[RoleMenus] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [RoleId] int NOT NULL,
    [MenuId] int NOT NULL,
    [AssignedDate] datetime2(7) NOT NULL DEFAULT GETDATE(),
    [IsActive] bit NOT NULL DEFAULT 1,
    [AssignedBy] int NULL,
    [Notes] nvarchar(500) NULL,
    CONSTRAINT [PK_RoleMenus] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RoleMenus_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [BigIntSoftware].[Roles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_RoleMenus_Menus_MenuId] FOREIGN KEY ([MenuId]) REFERENCES [BigIntSoftware].[Menus] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_RoleMenus_Users_AssignedBy] FOREIGN KEY ([AssignedBy]) REFERENCES [BigIntSoftware].[Users] ([Id]) ON DELETE NO ACTION
)
GO

-- Index'leri oluştur
CREATE UNIQUE INDEX [IX_UserMenus_UserId_MenuId] ON [BigIntSoftware].[UserMenus] ([UserId], [MenuId])
GO

CREATE UNIQUE INDEX [IX_UserMenuPermissions_UserId_MenuId_PermissionId] ON [BigIntSoftware].[UserMenuPermissions] ([UserId], [MenuId], [PermissionId])
GO

CREATE UNIQUE INDEX [IX_RoleMenuPermissions_RoleId_MenuId_PermissionId] ON [BigIntSoftware].[RoleMenuPermissions] ([RoleId], [MenuId], [PermissionId])
GO

CREATE UNIQUE INDEX [IX_UserRoles_UserId_RoleId] ON [BigIntSoftware].[UserRoles] ([UserId], [RoleId])
GO

CREATE UNIQUE INDEX [IX_RoleMenus_RoleId_MenuId] ON [BigIntSoftware].[RoleMenus] ([RoleId], [MenuId])
GO

PRINT 'Yeni tablo yapısı başarıyla oluşturuldu!'
PRINT 'Oluşturulan tablolar:'
PRINT '- UserMenus'
PRINT '- UserMenuPermissions'
PRINT '- RoleMenuPermissions'
PRINT '- UserRoles'
PRINT '- RoleMenus'
