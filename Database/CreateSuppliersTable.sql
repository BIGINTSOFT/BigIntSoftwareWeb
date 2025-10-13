-- Tedarikçi Tablosu Oluşturma Scripti
-- BigIntSoftware Schema'sında Suppliers tablosunu oluşturur

USE [bigintsoft]
GO

-- Schema kontrolü ve oluşturma
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'BigIntSoftware')
BEGIN
    EXEC('CREATE SCHEMA [BigIntSoftware]')
END
GO

-- Tedarikçi tablosunu oluştur
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[BigIntSoftware].[Suppliers]') AND type in (N'U'))
BEGIN
    CREATE TABLE [BigIntSoftware].[Suppliers] (
        [Id] int IDENTITY(1,1) NOT NULL,
        
        -- Temel Tedarikçi Bilgileri
        [CompanyName] nvarchar(100) NOT NULL,
        [FirstName] nvarchar(50) NULL,
        [LastName] nvarchar(50) NULL,
        [ContactPerson] nvarchar(100) NULL,
        [ContactPersonTitle] nvarchar(100) NULL,
        [ContactPersonDepartment] nvarchar(100) NULL,
        
        -- SAP Entegrasyon Alanları
        [SapVendorCode] nvarchar(20) NULL,
        [SapAccountGroup] nvarchar(20) NULL,
        [SapPurchasingOrg] nvarchar(20) NULL,
        [SapCompanyCode] nvarchar(20) NULL,
        [SapPaymentTerms] nvarchar(20) NULL,
        [SapCurrency] nvarchar(20) NULL,
        [SapTaxCode] nvarchar(20) NULL,
        [SapTaxCountry] nvarchar(20) NULL,
        
        -- İletişim Bilgileri
        [Phone] nvarchar(20) NULL,
        [MobilePhone] nvarchar(20) NULL,
        [Fax] nvarchar(20) NULL,
        [Email] nvarchar(100) NULL,
        [Website] nvarchar(100) NULL,
        [LinkedIn] nvarchar(100) NULL,
        [Twitter] nvarchar(100) NULL,
        [Facebook] nvarchar(100) NULL,
        [Instagram] nvarchar(100) NULL,
        
        -- Adres Bilgileri
        [Address] nvarchar(200) NULL,
        [City] nvarchar(50) NULL,
        [State] nvarchar(50) NULL,
        [PostalCode] nvarchar(20) NULL,
        [Country] nvarchar(50) NULL,
        [Region] nvarchar(50) NULL,
        [District] nvarchar(50) NULL,
        
        -- Teslimat Adresi
        [DeliveryAddress] nvarchar(200) NULL,
        [DeliveryCity] nvarchar(50) NULL,
        [DeliveryState] nvarchar(50) NULL,
        [DeliveryPostalCode] nvarchar(20) NULL,
        [DeliveryCountry] nvarchar(50) NULL,
        
        -- Fatura Adresi
        [BillingAddress] nvarchar(200) NULL,
        [BillingCity] nvarchar(50) NULL,
        [BillingState] nvarchar(50) NULL,
        [BillingPostalCode] nvarchar(20) NULL,
        [BillingCountry] nvarchar(50) NULL,
        
        -- Vergi ve Resmi Bilgiler (Türkiye E-Fatura/E-İrsaliye)
        [TcNumber] nvarchar(11) NULL,
        [TaxNumber] nvarchar(10) NULL,
        [TaxOffice] nvarchar(50) NULL,
        [TaxOfficeCode] nvarchar(50) NULL,
        [EInvoiceAlias] nvarchar(20) NULL,
        [EInvoiceTitle] nvarchar(50) NULL,
        [EInvoiceProfile] nvarchar(20) NULL,
        [EArchiveProfile] nvarchar(20) NULL,
        [EDeliveryNoteProfile] nvarchar(20) NULL,
        [EInvoiceTestAlias] nvarchar(20) NULL,
        [EInvoiceProdAlias] nvarchar(20) NULL,
        
        -- Avrupa ve Uluslararası Bilgiler
        [VatNumber] nvarchar(20) NULL,
        [VatCountryCode] nvarchar(10) NULL,
        [LegalEntityType] nvarchar(50) NULL,
        [SupplierType] nvarchar(50) NULL,
        [SupplierCategory] nvarchar(50) NULL,
        [SupplierGroup] nvarchar(50) NULL,
        [SupplierSegment] nvarchar(50) NULL,
        
        -- Banka Bilgileri
        [BankName] nvarchar(50) NULL,
        [BankBranch] nvarchar(50) NULL,
        [BankAccountNumber] nvarchar(30) NULL,
        [Iban] nvarchar(20) NULL,
        [SwiftCode] nvarchar(20) NULL,
        [BankAddress] nvarchar(50) NULL,
        [BankCity] nvarchar(50) NULL,
        [BankCountry] nvarchar(50) NULL,
        
        -- Ticari Bilgiler
        [TradeRegistryNumber] nvarchar(50) NULL,
        [ChamberOfCommerce] nvarchar(50) NULL,
        [MersisNumber] nvarchar(50) NULL,
        [ActivityCode] nvarchar(50) NULL,
        [ActivityDescription] nvarchar(200) NULL,
        [IndustryCode] nvarchar(50) NULL,
        [IndustryDescription] nvarchar(100) NULL,
        
        -- Fatura ve Ödeme Bilgileri
        [PaymentMethod] nvarchar(20) NULL,
        [PaymentTermDays] int NULL,
        [CreditLimit] decimal(18,2) NULL,
        [DiscountRate] decimal(5,2) NULL,
        [Currency] nvarchar(20) NOT NULL DEFAULT 'TRY',
        [PaymentCurrency] nvarchar(20) NULL,
        [ExchangeRate] decimal(5,2) NULL,
        
        -- E-Fatura ve E-Arşiv Bilgileri
        [IsEInvoiceEnabled] bit NOT NULL DEFAULT 0,
        [IsEArchiveEnabled] bit NOT NULL DEFAULT 0,
        [IsEDeliveryNoteEnabled] bit NOT NULL DEFAULT 0,
        [IsEInvoiceTestMode] bit NOT NULL DEFAULT 1,
        [IsEArchiveTestMode] bit NOT NULL DEFAULT 1,
        [IsEDeliveryNoteTestMode] bit NOT NULL DEFAULT 1,
        [EInvoiceIntegrationType] nvarchar(50) NULL,
        [EInvoiceServiceProvider] nvarchar(50) NULL,
        [EInvoiceApiUrl] nvarchar(100) NULL,
        [EInvoiceUsername] nvarchar(100) NULL,
        [EInvoicePassword] nvarchar(100) NULL,
        [EInvoiceCertificatePath] nvarchar(100) NULL,
        [EInvoiceCertificatePassword] nvarchar(100) NULL,
        
        -- Kalite ve Sertifikasyon
        [QualityCertification] nvarchar(50) NULL,
        [IsoCertification] nvarchar(50) NULL,
        [EnvironmentalCertification] nvarchar(50) NULL,
        [SafetyCertification] nvarchar(50) NULL,
        [CertificationExpiryDate] datetime2 NULL,
        
        -- Performans ve Değerlendirme
        [QualityRating] decimal(3,2) NULL,
        [DeliveryRating] decimal(3,2) NULL,
        [ServiceRating] decimal(3,2) NULL,
        [PriceRating] decimal(3,2) NULL,
        [OverallRating] decimal(3,2) NULL,
        [RiskLevel] nvarchar(20) NULL,
        [CreditRating] nvarchar(20) NULL,
        [IsBlacklisted] bit NOT NULL DEFAULT 0,
        [BlacklistReason] nvarchar(500) NULL,
        [BlacklistDate] datetime2 NULL,
        
        -- Sistem Bilgileri
        [IsActive] bit NOT NULL DEFAULT 1,
        [CreatedDate] datetime2 NULL DEFAULT GETDATE(),
        [UpdatedDate] datetime2 NULL,
        [CreatedBy] nvarchar(50) NULL,
        [UpdatedBy] nvarchar(50) NULL,
        [Notes] nvarchar(500) NULL,
        [InternalNotes] nvarchar(500) NULL,
        [QualityNotes] nvarchar(500) NULL,
        [PaymentNotes] nvarchar(500) NULL,
        
        -- Kategorizasyon
        [Source] nvarchar(50) NULL,
        [Priority] nvarchar(50) NULL,
        [Status] nvarchar(50) NULL,
        
        -- İstatistik Bilgileri
        [TotalOrders] int NULL,
        [TotalPurchases] decimal(18,2) NULL,
        [LastOrderDate] datetime2 NULL,
        [LastContactDate] datetime2 NULL,
        [LastDeliveryDate] datetime2 NULL,
        [LastPaymentDate] datetime2 NULL,
        
        -- Yasal Uyumluluk
        [GdprConsent] bit NOT NULL DEFAULT 0,
        [GdprConsentDate] datetime2 NULL,
        [MarketingConsent] bit NOT NULL DEFAULT 0,
        [SmsConsent] bit NOT NULL DEFAULT 0,
        [EmailConsent] bit NOT NULL DEFAULT 0,
        
        -- Dil ve Bölgesel Ayarlar
        [Language] nvarchar(10) NOT NULL DEFAULT 'tr-TR',
        [TimeZone] nvarchar(10) NOT NULL DEFAULT 'Turkey Standard Time',
        [DateFormat] nvarchar(10) NOT NULL DEFAULT 'dd.MM.yyyy',
        [NumberFormat] nvarchar(10) NOT NULL DEFAULT 'tr-TR',
        
        -- Ek SAP Alanları
        [SapVendorAccount] nvarchar(20) NULL,
        [SapReconciliationAccount] nvarchar(20) NULL,
        [SapSortKey] nvarchar(20) NULL,
        [SapPaymentBlock] nvarchar(20) NULL,
        [SapPostingBlock] nvarchar(20) NULL,
        [SapPurchasingBlock] nvarchar(20) NULL,
        [SapOrderCurrency] nvarchar(20) NULL,
        [SapIncoterms] nvarchar(20) NULL,
        [SapTransportationZone] nvarchar(20) NULL,
        [SapShippingCondition] nvarchar(20) NULL,
        [SapDeliveryPriority] nvarchar(20) NULL,
        [SapMinimumOrderValue] nvarchar(20) NULL,
        [SapMaximumOrderValue] nvarchar(20) NULL,
        
        -- E-İrsaliye Özel Alanları
        [EDeliveryNoteAlias] nvarchar(50) NULL,
        [EDeliveryNoteTitle] nvarchar(50) NULL,
        [EDeliveryNoteIntegrationType] nvarchar(50) NULL,
        [EDeliveryNoteServiceProvider] nvarchar(50) NULL,
        [EDeliveryNoteApiUrl] nvarchar(100) NULL,
        [EDeliveryNoteUsername] nvarchar(100) NULL,
        [EDeliveryNotePassword] nvarchar(100) NULL,
        
        -- Tedarikçi Özel Alanları
        [SupplierCode] nvarchar(50) NULL,
        [SupplierName] nvarchar(50) NULL,
        [SupplierShortName] nvarchar(50) NULL,
        [SupplierDisplayName] nvarchar(50) NULL,
        [SupplierLegalName] nvarchar(50) NULL,
        [SupplierCommercialName] nvarchar(50) NULL,
        [SupplierBrandName] nvarchar(50) NULL,
        
        -- İletişim Özel Alanları
        [PrimaryContactEmail] nvarchar(100) NULL,
        [SecondaryContactEmail] nvarchar(100) NULL,
        [PrimaryContactPhone] nvarchar(20) NULL,
        [SecondaryContactPhone] nvarchar(20) NULL,
        [EmergencyContactName] nvarchar(100) NULL,
        [EmergencyContactPhone] nvarchar(20) NULL,
        [EmergencyContactEmail] nvarchar(100) NULL,
        
        -- Finansal Bilgiler
        [AnnualRevenue] decimal(18,2) NULL,
        [NetWorth] decimal(18,2) NULL,
        [TotalAssets] decimal(18,2) NULL,
        [TotalLiabilities] decimal(18,2) NULL,
        [EmployeeCount] int NULL,
        [EstablishmentDate] datetime2 NULL,
        [OwnershipType] nvarchar(50) NULL,
        [BusinessSize] nvarchar(50) NULL,
        
        -- Tedarikçi Performans Metrikleri
        [OnTimeDeliveryRate] decimal(5,2) NULL,
        [QualityAcceptanceRate] decimal(5,2) NULL,
        [PriceCompetitiveness] decimal(5,2) NULL,
        [ServiceLevel] decimal(5,2) NULL,
        [AverageDeliveryDays] int NULL,
        [MinimumOrderQuantity] int NULL,
        [MaximumOrderQuantity] int NULL,
        [MinimumOrderValue] decimal(18,2) NULL,
        [MaximumOrderValue] decimal(18,2) NULL,
        
        -- Tedarikçi Sertifikaları ve Belgeleri
        [BusinessLicense] nvarchar(100) NULL,
        [TaxCertificate] nvarchar(100) NULL,
        [TradeRegistryCertificate] nvarchar(100) NULL,
        [ChamberOfCommerceCertificate] nvarchar(100) NULL,
        [InsuranceCertificate] nvarchar(100) NULL,
        [FinancialStatement] nvarchar(100) NULL,
        [BusinessLicenseExpiryDate] datetime2 NULL,
        [TaxCertificateExpiryDate] datetime2 NULL,
        [TradeRegistryExpiryDate] datetime2 NULL,
        [ChamberOfCommerceExpiryDate] datetime2 NULL,
        [InsuranceExpiryDate] datetime2 NULL,
        [FinancialStatementDate] datetime2 NULL,
        
        CONSTRAINT [PK_Suppliers] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
    
    PRINT 'Suppliers tablosu başarıyla oluşturuldu.'
END
ELSE
BEGIN
    PRINT 'Suppliers tablosu zaten mevcut.'
END
GO

-- Index'leri oluştur
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[BigIntSoftware].[Suppliers]') AND name = N'IX_Suppliers_CompanyName')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Suppliers_CompanyName] ON [BigIntSoftware].[Suppliers] ([CompanyName])
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[BigIntSoftware].[Suppliers]') AND name = N'IX_Suppliers_Email')
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Suppliers_Email] ON [BigIntSoftware].[Suppliers] ([Email]) 
    WHERE ([Email] IS NOT NULL AND [Email] != '')
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[BigIntSoftware].[Suppliers]') AND name = N'IX_Suppliers_TaxNumber')
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Suppliers_TaxNumber] ON [BigIntSoftware].[Suppliers] ([TaxNumber]) 
    WHERE ([TaxNumber] IS NOT NULL AND [TaxNumber] != '')
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[BigIntSoftware].[Suppliers]') AND name = N'IX_Suppliers_TcNumber')
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Suppliers_TcNumber] ON [BigIntSoftware].[Suppliers] ([TcNumber]) 
    WHERE ([TcNumber] IS NOT NULL AND [TcNumber] != '')
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[BigIntSoftware].[Suppliers]') AND name = N'IX_Suppliers_SapVendorCode')
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Suppliers_SapVendorCode] ON [BigIntSoftware].[Suppliers] ([SapVendorCode]) 
    WHERE ([SapVendorCode] IS NOT NULL AND [SapVendorCode] != '')
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[BigIntSoftware].[Suppliers]') AND name = N'IX_Suppliers_EInvoiceAlias')
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Suppliers_EInvoiceAlias] ON [BigIntSoftware].[Suppliers] ([EInvoiceAlias]) 
    WHERE ([EInvoiceAlias] IS NOT NULL AND [EInvoiceAlias] != '')
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[BigIntSoftware].[Suppliers]') AND name = N'IX_Suppliers_VatNumber')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Suppliers_VatNumber] ON [BigIntSoftware].[Suppliers] ([VatNumber])
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[BigIntSoftware].[Suppliers]') AND name = N'IX_Suppliers_IsActive')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Suppliers_IsActive] ON [BigIntSoftware].[Suppliers] ([IsActive])
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[BigIntSoftware].[Suppliers]') AND name = N'IX_Suppliers_SupplierType')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Suppliers_SupplierType] ON [BigIntSoftware].[Suppliers] ([SupplierType])
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[BigIntSoftware].[Suppliers]') AND name = N'IX_Suppliers_SupplierCategory')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Suppliers_SupplierCategory] ON [BigIntSoftware].[Suppliers] ([SupplierCategory])
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[BigIntSoftware].[Suppliers]') AND name = N'IX_Suppliers_SupplierGroup')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Suppliers_SupplierGroup] ON [BigIntSoftware].[Suppliers] ([SupplierGroup])
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[BigIntSoftware].[Suppliers]') AND name = N'IX_Suppliers_SupplierSegment')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Suppliers_SupplierSegment] ON [BigIntSoftware].[Suppliers] ([SupplierSegment])
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[BigIntSoftware].[Suppliers]') AND name = N'IX_Suppliers_RiskLevel')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Suppliers_RiskLevel] ON [BigIntSoftware].[Suppliers] ([RiskLevel])
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[BigIntSoftware].[Suppliers]') AND name = N'IX_Suppliers_CreditRating')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Suppliers_CreditRating] ON [BigIntSoftware].[Suppliers] ([CreditRating])
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[BigIntSoftware].[Suppliers]') AND name = N'IX_Suppliers_IsBlacklisted')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Suppliers_IsBlacklisted] ON [BigIntSoftware].[Suppliers] ([IsBlacklisted])
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[BigIntSoftware].[Suppliers]') AND name = N'IX_Suppliers_IsEInvoiceEnabled')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Suppliers_IsEInvoiceEnabled] ON [BigIntSoftware].[Suppliers] ([IsEInvoiceEnabled])
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[BigIntSoftware].[Suppliers]') AND name = N'IX_Suppliers_IsEArchiveEnabled')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Suppliers_IsEArchiveEnabled] ON [BigIntSoftware].[Suppliers] ([IsEArchiveEnabled])
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[BigIntSoftware].[Suppliers]') AND name = N'IX_Suppliers_IsEDeliveryNoteEnabled')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Suppliers_IsEDeliveryNoteEnabled] ON [BigIntSoftware].[Suppliers] ([IsEDeliveryNoteEnabled])
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[BigIntSoftware].[Suppliers]') AND name = N'IX_Suppliers_CreatedDate')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Suppliers_CreatedDate] ON [BigIntSoftware].[Suppliers] ([CreatedDate])
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[BigIntSoftware].[Suppliers]') AND name = N'IX_Suppliers_CertificationExpiryDate')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Suppliers_CertificationExpiryDate] ON [BigIntSoftware].[Suppliers] ([CertificationExpiryDate])
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[BigIntSoftware].[Suppliers]') AND name = N'IX_Suppliers_LastOrderDate')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Suppliers_LastOrderDate] ON [BigIntSoftware].[Suppliers] ([LastOrderDate])
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[BigIntSoftware].[Suppliers]') AND name = N'IX_Suppliers_LastDeliveryDate')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Suppliers_LastDeliveryDate] ON [BigIntSoftware].[Suppliers] ([LastDeliveryDate])
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[BigIntSoftware].[Suppliers]') AND name = N'IX_Suppliers_LastPaymentDate')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Suppliers_LastPaymentDate] ON [BigIntSoftware].[Suppliers] ([LastPaymentDate])
END
GO

PRINT 'Tüm indexler başarıyla oluşturuldu.'
GO

-- Test verisi ekle (isteğe bağlı)
IF NOT EXISTS (SELECT * FROM [BigIntSoftware].[Suppliers] WHERE [CompanyName] = 'Test Tedarikçi')
BEGIN
    INSERT INTO [BigIntSoftware].[Suppliers] (
        [CompanyName], [SupplierType], [SupplierCategory], [IsActive], [Currency], [Language]
    ) VALUES (
        'Test Tedarikçi', 'Ana Tedarikçi', 'Malzeme', 1, 'TRY', 'tr-TR'
    )
    
    PRINT 'Test verisi eklendi.'
END
GO

PRINT 'Suppliers tablosu ve tüm bileşenleri başarıyla oluşturuldu!'
GO
