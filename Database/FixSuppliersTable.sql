-- Tedarikçi Tablosu Düzeltme Scripti
-- Index sorunlarını ve TimeZone alanını düzeltir

USE [bigintsoft]
GO

-- TimeZone alanını genişlet
ALTER TABLE [BigIntSoftware].[Suppliers] 
ALTER COLUMN [TimeZone] nvarchar(50) NOT NULL DEFAULT 'Turkey Standard Time'
GO

-- Mevcut problemli index'leri sil
IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[BigIntSoftware].[Suppliers]') AND name = N'IX_Suppliers_Email')
BEGIN
    DROP INDEX [IX_Suppliers_Email] ON [BigIntSoftware].[Suppliers]
END
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[BigIntSoftware].[Suppliers]') AND name = N'IX_Suppliers_TaxNumber')
BEGIN
    DROP INDEX [IX_Suppliers_TaxNumber] ON [BigIntSoftware].[Suppliers]
END
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[BigIntSoftware].[Suppliers]') AND name = N'IX_Suppliers_TcNumber')
BEGIN
    DROP INDEX [IX_Suppliers_TcNumber] ON [BigIntSoftware].[Suppliers]
END
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[BigIntSoftware].[Suppliers]') AND name = N'IX_Suppliers_SapVendorCode')
BEGIN
    DROP INDEX [IX_Suppliers_SapVendorCode] ON [BigIntSoftware].[Suppliers]
END
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[BigIntSoftware].[Suppliers]') AND name = N'IX_Suppliers_EInvoiceAlias')
BEGIN
    DROP INDEX [IX_Suppliers_EInvoiceAlias] ON [BigIntSoftware].[Suppliers]
END
GO

-- Index'leri yeniden oluştur (SET QUOTED_IDENTIFIER ON ile)
SET QUOTED_IDENTIFIER ON
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Suppliers_Email] ON [BigIntSoftware].[Suppliers] ([Email]) 
WHERE ([Email] IS NOT NULL AND [Email] != '')
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Suppliers_TaxNumber] ON [BigIntSoftware].[Suppliers] ([TaxNumber]) 
WHERE ([TaxNumber] IS NOT NULL AND [TaxNumber] != '')
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Suppliers_TcNumber] ON [BigIntSoftware].[Suppliers] ([TcNumber]) 
WHERE ([TcNumber] IS NOT NULL AND [TcNumber] != '')
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Suppliers_SapVendorCode] ON [BigIntSoftware].[Suppliers] ([SapVendorCode]) 
WHERE ([SapVendorCode] IS NOT NULL AND [SapVendorCode] != '')
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Suppliers_EInvoiceAlias] ON [BigIntSoftware].[Suppliers] ([EInvoiceAlias]) 
WHERE ([EInvoiceAlias] IS NOT NULL AND [EInvoiceAlias] != '')
GO

-- Test verisini güncelle
UPDATE [BigIntSoftware].[Suppliers] 
SET [TimeZone] = 'Turkey Standard Time'
WHERE [CompanyName] = 'Test Tedarikçi'
GO

PRINT 'Suppliers tablosu düzeltmeleri tamamlandı!'
GO
