-- BigInt Software Veritabanı Tablo Oluşturma Scripti
-- Server: 77.245.159.27
-- Database: bigintsoft
-- Schema: BigIntSoftware

USE bigintsoft;
GO

-- Schema oluştur (eğer yoksa)
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'BigIntSoftware')
BEGIN
    EXEC('CREATE SCHEMA BigIntSoftware')
END
GO

-- Users tablosu oluştur
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[BigIntSoftware].[Users]') AND type in (N'U'))
BEGIN
    CREATE TABLE [BigIntSoftware].[Users] (
        [Id] int IDENTITY(1,1) NOT NULL,
        [Username] nvarchar(50) NOT NULL,
        [Email] nvarchar(100) NOT NULL,
        [Password] nvarchar(255) NOT NULL,
        [FirstName] nvarchar(50) NULL,
        [LastName] nvarchar(50) NULL,
        [IsActive] bit NOT NULL DEFAULT(1),
        [CreatedDate] datetime2 NOT NULL DEFAULT(GETDATE()),
        [LastLoginDate] datetime2 NULL,
        
        CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END
GO

-- Unique index'ler oluştur
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[BigIntSoftware].[Users]') AND name = N'IX_Users_Username')
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_Username] ON [BigIntSoftware].[Users] ([Username] ASC)
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[BigIntSoftware].[Users]') AND name = N'IX_Users_Email')
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_Email] ON [BigIntSoftware].[Users] ([Email] ASC)
END
GO

-- Test verilerini ekle
IF NOT EXISTS (SELECT * FROM [BigIntSoftware].[Users] WHERE Username = 'admin')
BEGIN
    INSERT INTO [BigIntSoftware].[Users] ([Username], [Email], [Password], [FirstName], [LastName], [IsActive], [CreatedDate])
    VALUES ('admin', 'admin@bigintsoft.com', 'jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=', 'Admin', 'User', 1, GETDATE())
END
GO

IF NOT EXISTS (SELECT * FROM [BigIntSoftware].[Users] WHERE Username = 'test')
BEGIN
    INSERT INTO [BigIntSoftware].[Users] ([Username], [Email], [Password], [FirstName], [LastName], [IsActive], [CreatedDate])
    VALUES ('test', 'test@bigintsoft.com', 'L7M1of5GP0qxqTz+R8SEf5KzUqk8v1t4WcRcFf0wGjc=', 'Test', 'User', 1, GETDATE())
END
GO

-- Tablo yapısını kontrol et
SELECT 
    TABLE_SCHEMA,
    TABLE_NAME,
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_SCHEMA = 'BigIntSoftware' 
    AND TABLE_NAME = 'Users'
ORDER BY ORDINAL_POSITION;
GO

-- Test verilerini kontrol et
SELECT * FROM [BigIntSoftware].[Users];
GO

PRINT 'BigInt Software veritabanı tabloları başarıyla oluşturuldu!';
PRINT 'Test kullanıcıları:';
PRINT 'Admin: admin / admin123';
PRINT 'Test: test / test123';
