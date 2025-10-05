# BigInt Software ERP - Kurumsal Kaynak Planlama Sistemi

Bu proje, .NET 8 kullanılarak geliştirilmiş katmanlı mimari (Layered Architecture) ile kurumsal kaynak planlama (ERP) sistemidir.

## Proje Yapısı

### Katmanlar
- **Entities**: Veri modelleri ve DTO'lar
- **DataAccess**: Veritabanı erişimi ve DbContext
- **Business**: Repository pattern ve Generic Repository
- **Web**: ASP.NET Core MVC UI katmanı
- **Api**: API katmanı (gelecekte geliştirilecek)

### Özellikler
- ✅ Kullanıcı giriş sistemi
- ✅ Cookie tabanlı authentication
- ✅ Generic Repository Pattern
- ✅ Dependency Injection
- ✅ Entity Framework Core
- ✅ SQL Server veritabanı
- ✅ Tailwind CSS UI
- ✅ Modern ve responsive tasarım
- ✅ ERP modül yapısı
- ✅ Kurumsal seviye güvenlik

## Veritabanı Bağlantısı

```
Server: 77.245.159.27
Database: bigintsoft
Schema: BigIntSoftware
User: biguser
Password: Bigboys2024*
```

## Test Kullanıcıları

Uygulama ilk çalıştırıldığında otomatik olarak oluşturulan test kullanıcıları:

1. **Admin Kullanıcısı**
   - Kullanıcı Adı: `admin`
   - Şifre: `admin123`
   - E-posta: `admin@bigintsoft.com`

2. **Test Kullanıcısı**
   - Kullanıcı Adı: `test`
   - Şifre: `test123`
   - E-posta: `test@bigintsoft.com`

## Kurulum ve Çalıştırma

### 1. Veritabanı Kurulumu
```sql
-- SQL Server Management Studio'da Database/CreateTables.sql dosyasını çalıştırın
-- Veya aşağıdaki komutları kullanın:
```

### 2. Proje Kurulumu
1. Projeyi klonlayın
2. `dotnet restore` komutu ile paketleri yükleyin
3. Tailwind CSS'i derleyin:
   ```bash
   cd Web
   npm install
   npm run build-css
   ```
4. `dotnet run --project Web` komutu ile uygulamayı çalıştırın
5. Tarayıcıda `https://localhost:5001` adresine gidin

### 3. Tailwind CSS Geliştirme
Geliştirme sırasında Tailwind CSS'i watch modunda çalıştırmak için:
```bash
cd Web
npm run build-css
```

## Geliştirme Notları

- Şifreler SHA256 ile hash'lenmiştir (production'da BCrypt kullanılmalı)
- Generic Repository pattern kullanılmaktadır
- Tailwind CSS ile modern UI tasarımı
- Cookie authentication kullanılmaktadır
- Responsive tasarım (mobil uyumlu)
- Veritabanından doğrudan veri çekilir (SeedData kullanılmaz)

## Repository Pattern

### Generic Repository
```csharp
// Temel CRUD işlemleri
await repository.GetByIdAsync(1);
await repository.GetAllAsync();
await repository.AddAsync(entity);
await repository.UpdateAsync(entity);
await repository.DeleteAsync(1);
```

### Özel Repository Metodları
```csharp
// UserRepository'de özel metodlar
await userRepository.GetByUsernameAsync("admin");
await userRepository.LoginAsync("admin", "password");
```

## Gelecek Geliştirmeler

- [ ] Şifre sıfırlama
- [ ] Kullanıcı kayıt sistemi
- [ ] Rol tabanlı yetkilendirme
- [ ] API katmanı geliştirme
- [ ] Loglama sistemi
- [ ] Unit testler
- [ ] Dark mode desteği
