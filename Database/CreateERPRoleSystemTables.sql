-- =============================================
-- ERP Standartlarına Uygun Rol ve Yetki Sistemi
-- Profesyonel RBAC (Role-Based Access Control) Yapısı
-- =============================================

USE [BigIntSoftwareDB]
GO

-- =============================================
-- 1. MEVCUT İLİŞKİ TABLOLARINI TEMİZLE
-- =============================================

-- Mevcut ilişki tablolarını temizle
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[BigIntSoftware].[UserRoles]') AND type in (N'U'))
    DROP TABLE [BigIntSoftware].[UserRoles]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[BigIntSoftware].[RolePermissions]') AND type in (N'U'))
    DROP TABLE [BigIntSoftware].[RolePermissions]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[BigIntSoftware].[UserPermissions]') AND type in (N'U'))
    DROP TABLE [BigIntSoftware].[UserPermissions]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[BigIntSoftware].[RoleMenus]') AND type in (N'U'))
    DROP TABLE [BigIntSoftware].[RoleMenus]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[BigIntSoftware].[UserMenus]') AND type in (N'U'))
    DROP TABLE [BigIntSoftware].[UserMenus]
GO

-- =============================================
-- 2. YENİ İLİŞKİ TABLOLARINI OLUŞTUR
-- =============================================

-- UserRoles: Kullanıcı-Rol İlişkisi
CREATE TABLE [BigIntSoftware].[UserRoles] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserId] int NOT NULL,
    [RoleId] int NOT NULL,
    [AssignedDate] datetime2(7) NOT NULL DEFAULT GETDATE(),
    [ExpiryDate] datetime2(7) NULL,
    [IsActive] bit NOT NULL DEFAULT 1,
    [AssignedBy] int NULL,
    [Notes] nvarchar(500) NULL,
    CONSTRAINT [PK_UserRoles] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserRoles_Users] FOREIGN KEY ([UserId]) REFERENCES [BigIntSoftware].[Users] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserRoles_Roles] FOREIGN KEY ([RoleId]) REFERENCES [BigIntSoftware].[Roles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserRoles_AssignedBy] FOREIGN KEY ([AssignedBy]) REFERENCES [BigIntSoftware].[Users] ([Id]),
    CONSTRAINT [UQ_UserRoles_User_Role] UNIQUE ([UserId], [RoleId])
)
GO

-- RoleMenuPermissions: Rol-Menü-Yetki Seviyesi İlişkisi (Ana Yetki Yapısı)
CREATE TABLE [BigIntSoftware].[RoleMenuPermissions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [RoleId] int NOT NULL,
    [MenuId] int NOT NULL,
    [PermissionLevel] nvarchar(20) NOT NULL, -- VIEW, CREATE, EDIT, DELETE
    [AssignedDate] datetime2(7) NOT NULL DEFAULT GETDATE(),
    [IsActive] bit NOT NULL DEFAULT 1,
    [AssignedBy] int NULL,
    [Notes] nvarchar(500) NULL,
    CONSTRAINT [PK_RoleMenuPermissions] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_RoleMenuPermissions_Roles] FOREIGN KEY ([RoleId]) REFERENCES [BigIntSoftware].[Roles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_RoleMenuPermissions_Menus] FOREIGN KEY ([MenuId]) REFERENCES [BigIntSoftware].[Menus] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_RoleMenuPermissions_AssignedBy] FOREIGN KEY ([AssignedBy]) REFERENCES [BigIntSoftware].[Users] ([Id]),
    CONSTRAINT [UQ_RoleMenuPermissions_Role_Menu] UNIQUE ([RoleId], [MenuId]),
    CONSTRAINT [CK_RoleMenuPermissions_PermissionLevel] CHECK ([PermissionLevel] IN ('VIEW', 'CREATE', 'EDIT', 'DELETE'))
)
GO

-- UserExtraPermissions: Kullanıcı Ekstra Yetkileri (İstisna Durumlar)
CREATE TABLE [BigIntSoftware].[UserExtraPermissions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserId] int NOT NULL,
    [MenuId] int NOT NULL,
    [PermissionLevel] nvarchar(20) NOT NULL, -- VIEW, CREATE, EDIT, DELETE
    [Reason] nvarchar(200) NOT NULL, -- Neden verildi
    [AssignedDate] datetime2(7) NOT NULL DEFAULT GETDATE(),
    [ExpiryDate] datetime2(7) NULL, -- Ne zaman sona erer
    [IsActive] bit NOT NULL DEFAULT 1,
    [AssignedBy] int NULL,
    [Notes] nvarchar(500) NULL,
    CONSTRAINT [PK_UserExtraPermissions] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserExtraPermissions_Users] FOREIGN KEY ([UserId]) REFERENCES [BigIntSoftware].[Users] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserExtraPermissions_Menus] FOREIGN KEY ([MenuId]) REFERENCES [BigIntSoftware].[Menus] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserExtraPermissions_AssignedBy] FOREIGN KEY ([AssignedBy]) REFERENCES [BigIntSoftware].[Users] ([Id]),
    CONSTRAINT [UQ_UserExtraPermissions_User_Menu] UNIQUE ([UserId], [MenuId]),
    CONSTRAINT [CK_UserExtraPermissions_PermissionLevel] CHECK ([PermissionLevel] IN ('VIEW', 'CREATE', 'EDIT', 'DELETE'))
)
GO

-- =============================================
-- 3. İNDEXLER OLUŞTUR
-- =============================================

-- UserRoles Indexleri
CREATE NONCLUSTERED INDEX [IX_UserRoles_UserId] ON [BigIntSoftware].[UserRoles] ([UserId])
GO
CREATE NONCLUSTERED INDEX [IX_UserRoles_RoleId] ON [BigIntSoftware].[UserRoles] ([RoleId])
GO
CREATE NONCLUSTERED INDEX [IX_UserRoles_IsActive] ON [BigIntSoftware].[UserRoles] ([IsActive])
GO

-- RoleMenuPermissions Indexleri
CREATE NONCLUSTERED INDEX [IX_RoleMenuPermissions_RoleId] ON [BigIntSoftware].[RoleMenuPermissions] ([RoleId])
GO
CREATE NONCLUSTERED INDEX [IX_RoleMenuPermissions_MenuId] ON [BigIntSoftware].[RoleMenuPermissions] ([MenuId])
GO
CREATE NONCLUSTERED INDEX [IX_RoleMenuPermissions_IsActive] ON [BigIntSoftware].[RoleMenuPermissions] ([IsActive])
GO

-- UserExtraPermissions Indexleri
CREATE NONCLUSTERED INDEX [IX_UserExtraPermissions_UserId] ON [BigIntSoftware].[UserExtraPermissions] ([UserId])
GO
CREATE NONCLUSTERED INDEX [IX_UserExtraPermissions_MenuId] ON [BigIntSoftware].[UserExtraPermissions] ([MenuId])
GO
CREATE NONCLUSTERED INDEX [IX_UserExtraPermissions_IsActive] ON [BigIntSoftware].[UserExtraPermissions] ([IsActive])
GO
CREATE NONCLUSTERED INDEX [IX_UserExtraPermissions_ExpiryDate] ON [BigIntSoftware].[UserExtraPermissions] ([ExpiryDate])
GO

-- =============================================
-- 4. BAŞARILI MESAJI
-- =============================================
PRINT 'ERP Standartlarına Uygun Rol ve Yetki Sistemi tabloları başarıyla oluşturuldu!'
PRINT 'Tablolar:'
PRINT '- UserRoles: Kullanıcı-Rol İlişkisi'
PRINT '- RoleMenuPermissions: Rol-Menü-Yetki Seviyesi (Ana Yetki Yapısı)'
PRINT '- UserExtraPermissions: Kullanıcı Ekstra Yetkileri (İstisna Durumlar)'
GO
