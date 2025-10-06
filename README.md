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

## RBAC İyileştirme ve Geçiş Planı (Seçenek 1)

Amaç: Menü bazlı ve sistem (global) yetkilerini sade ve tutarlı bir modelle yönetmek. Kullanıcılar ve roller yetkileri, menü-permission eşlemesine (MenuPermission) dayanarak verilir. Global yetkiler için özel bir GLOBAL menü kaydı kullanılır.

### 1) Yeni Veri Modeli
- MenuPermission: MenuId + PermissionId eşlemesi (aktif/pasif)
- RoleMenuPermission: RoleId + MenuPermissionId (rolün menü-permission’a sahipliği)
- UserMenuPermission: UserId + MenuPermissionId (kullanıcının menü-permission’a sahipliği)

Not: GLOBAL yetkiler için `Menu` tablosunda özel bir kayıt (örn. Id=0 veya ayrı bir satır) kullanılacak. Böylece `MenuId=NULL` karmaşası olmadan global/sistem yetkileri yönetilir.

### 2) Migration Adımları (Veri Tabanı)
1. Yeni tabloları oluştur:
   - MenuPermissions(Id, MenuId, PermissionId, IsActive, CreatedDate)
   - RoleMenuPermissions(Id, RoleId, MenuPermissionId, IsActive, AssignedDate)
   - UserMenuPermissions(Id, UserId, MenuPermissionId, IsActive, AssignedDate, ExpiryDate)
2. GLOBAL menü kaydını ekle (örn. Name="GLOBAL", IsVisible=false).
3. Mevcut veriyi taşı:
   - RolePermission.MenuId IS NULL → MenuPermission(Menu=GLOBAL, PermissionId)
   - RolePermission.MenuId NOT NULL → MenuPermission(Menu=MenuId, PermissionId)
   - Benzer şekilde UserPermission kayıtlarını UserMenuPermission’a taşı
4. Eski tabloları (RolePermission, UserPermission) geçici olarak read-only bırak; temizlik sonraya.

### 3) Repository Katmanı Güncellemeleri
- IPermissionRepository değişiklikleri minimal tutulur; ana akış MenuPermission üzerinden çalışır.
- Yeni metod örnekleri:
  - GetMenuPermissions(menuId)
  - GetGlobalPermissions()
  - GetRoleMenuPermissions(roleId, menuId or GLOBAL)
  - AssignMenuPermissionToRole(roleId, menuPermissionId)
  - RemoveMenuPermissionFromRole(roleId, menuPermissionId)
  - Aynılarının User versiyonları
- HasPermission(userId, code, controller, action) akışı:
  1) Route’tan menuId çöz
  2) Global (GLOBAL menü) + Menü-spesifik izinleri birlikte kontrol et

### 4) Servis/Controller Değişiklikleri
- Mevcut endpoint’ler yeni repository metodlarına yönlendirilir.
- Sistem Yetkileri ekranı GLOBAL menüye bağlı MenuPermission setlerini kullanır.
- Menü Yetkileri ekranı seçilen Menü için MenuPermission’ları listeler.

### 5) UI/UX ve JS Güncellemeleri
- Role modalında 3 TAB korunur: Kullanıcılar, Menü Yetkileri, Sistem Yetkileri.
- Sistem Yetkileri: GLOBAL menüye ait MenuPermission listelenir.
- Menü Yetkileri: Seçili menünün MenuPermission listesi.
- Ekle/Kaldır işlemleri `MenuPermissionId` üzerinden yapılır.

### 6) Geçiş Sırası (Önerilen Yol Haritası)
1. DB migration ve yeni tabloların eklenmesi
2. Veri taşıma scriptleri (RolePermission/UserPermission → RoleMenuPermission/UserMenuPermission + MenuPermission)
3. Repository güncellemeleri ve yeni metodların eklenmesi
4. Controller endpointlerinin yeni yapıya uyarlanması
5. UI/JS güncellemeleri (Role, Permission, Menu sayfaları)
6. Eski tabloların kaldırılması veya arşivlenmesi

### 7) Geri Dönüş ve Test
- Unit/integration test ile HasPermission akışları doğrulanır:
  - Global VIEW ile tüm menüler görünüyor mu?
  - Menü-spesifik yetki öncelikleri doğru mu?
  - Rol ve kullanıcıya doğrudan atama çakışmalarında beklenen sonuç alınıyor mu?

### 8) Performans ve Index Önerileri
- MenuPermission: (MenuId, PermissionId) unique index
- RoleMenuPermission: (RoleId, MenuPermissionId) unique index
- UserMenuPermission: (UserId, MenuPermissionId) unique index
- Sık kullanılan kolonlara (IsActive, MenuId, PermissionId, RoleId, UserId) index

### 9) Güvenlik ve Temizlik
- Atama/kaldırma endpointlerinde yetki kontrolü (EDIT/ADMIN) zorunlu
- Soft delete mi hard delete mi? İş kurallarına göre belirlenmeli (öneri: soft delete → IsActive)

Bu planla, menü ve sistem yetkileri net, genişletilebilir ve UI’dan kolay yönetilebilir bir yapıya kavuşur. Kodlamaya başlamadan önce bu adımlar sırayla uygulanmalıdır.