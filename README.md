# 📚 Kitap Dünyası

ASP.NET Core MVC ile geliştirilmiş bir **online kitap satış (e-ticaret) uygulaması**.
MCSD Yazılım Uzmanlığı bitirme projesi olarak hazırlanmıştır.

## Kullanılan Teknolojiler

- ASP.NET Core MVC (.NET 10)
- ASP.NET Core Web API + Swagger
- Entity Framework Core (Code First, Migration)
- SQL Server
- ASP.NET Core Identity (üyelik ve rol yönetimi)
- Bootstrap 5 (responsive arayüz)

## Proje Yapısı

Proje tek bir solution içinde üç katmandan oluşur:

| Proje | Açıklama |
|---|---|
| **KitapSatis.Data** | Entity'ler, DbContext, Generic Repository, Migration'lar ve seed verileri |
| **KitapSatis.Web** | MVC uygulaması: mağaza sayfaları ve Admin paneli |
| **KitapSatis.Api** | Web API: Category için CRUD endpointleri |

MVC ve API projeleri aynı Data katmanını kullanır.

## Mimari

- **Generic Repository Pattern:** Tüm entity'ler için ortak `IRepository<T>` / `Repository<T>`
- **Dependency Injection:** Repository'ler `Program.cs` içinde kaydedilir, controller'lara constructor üzerinden verilir
- **SOLID** prensiplerine uygun katmanlı yapı
- **ViewModel / DTO** kullanımı (form ve API verileri entity'den ayrı tutulur)

## Özellikler

**Müşteri tarafı**
- Ürün listeleme, kategoriye göre filtreleme, arama ve fiyata göre sıralama
- Ürün detay sayfası
- Üyelik: kayıt, giriş, çıkış, profil sayfası
- Sepet: ekleme, adet güncelleme, çıkarma (stok kontrolü ile)
- Sipariş oluşturma ve demo ödeme formu
- Sipariş geçmişi ve sipariş detayı

**Admin paneli** (sadece Admin rolü)
- Kategori yönetimi (CRUD)
- Ürün yönetimi (CRUD)
- Sipariş yönetimi (listeleme, detay, durum güncelleme)
- Kullanıcı yönetimi (listeleme, admin yetkisi verme/kaldırma, silme)

**Web API**
| Metot | Adres | Açıklama |
|---|---|---|
| GET | `/api/categories` | Tüm kategoriler |
| GET | `/api/categories/{id}` | Tek kategori |
| POST | `/api/categories` | Kategori ekle |
| PUT | `/api/categories/{id}` | Kategori güncelle |
| DELETE | `/api/categories/{id}` | Kategori sil |

## Kurulum

**Gereksinimler:** .NET 10 SDK ve SQL Server

1. Gerekirse `KitapSatis.Web/appsettings.json` ve `KitapSatis.Api/appsettings.json` içindeki
   `DefaultConnection` değerinde `Server=localhost` kısmını kendi SQL Server adınızla değiştirin.

2. EF Core aracını kurun (bir kez):