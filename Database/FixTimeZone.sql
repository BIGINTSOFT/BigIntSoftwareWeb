-- TimeZone alanını düzelt
USE [bigintsoft]
GO

-- TimeZone alanını genişlet
ALTER TABLE [BigIntSoftware].[Suppliers] 
ALTER COLUMN [TimeZone] nvarchar(50)
GO

-- Mevcut veriyi güncelle
UPDATE [BigIntSoftware].[Suppliers] 
SET [TimeZone] = 'Turkey Standard Time'
WHERE [TimeZone] = 'Turkey Sta'
GO

PRINT 'TimeZone alanı düzeltildi!'
GO
