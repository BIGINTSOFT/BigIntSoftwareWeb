-- Doğru hash ile güncelle
USE bigintsoft;
GO

-- Admin kullanıcısı için doğru hash
UPDATE [BigIntSoftware].[Users] 
SET Password = 'JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3THIKk=' 
WHERE Username = 'admin';
GO

-- Test kullanıcısı için de aynı şekilde hash'leyelim
-- test123 için hash hesapla (C# kodundaki HashPassword metodu ile)
UPDATE [BigIntSoftware].[Users] 
SET Password = 'L7M1of5GP0qxqTz+R8SEf5KzUqk8v1t4WcRcFf0wGjc=' 
WHERE Username = 'test';
GO

-- Kontrol et
SELECT Username, Password FROM [BigIntSoftware].[Users];
GO

PRINT 'Hash''ler güncellendi!';
PRINT 'Admin hash: JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3THIKk=';
PRINT 'Test hash: L7M1of5GP0qxqTz+R8SEf5KzUqk8v1t4WcRcFf0wGjc=';
