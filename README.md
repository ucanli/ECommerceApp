
# E-Commerce Payment Integration Backend

E-Commerce Payment Integration Challenge. 
Balance Management servisi ile entegre olan  ürün listeleme, sipariş oluşturma ve tamamlanmasını sağlayan uygulama.

## Proje Detayları

- **Backend**: .NET Core
- **Veritabanı**: In-Memory Sqlite
- **API Entegrasyonları**: Balance Management API
- **Containerization**: Docker & Docker Compose

## Gereksinimler

Bu projeyi çalıştırmak için aşağıdaki araçlara sahip olmanız gerekir:

- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://docs.docker.com/compose/install/)
- .NET SDK (Uygulama geliştirmek veya test etmek için)

## Başlangıç

### Docker Compose ile Çalıştırma

Docker üzerinde uygulamayı ayağa kaldırmak için

```bash
docker-compose up --build
```

Bu komut, aşağıdaki servisi başlatır:

- **ECommerce.API**: http://localhost:5001
- **API Dökümanı**: http://localhost:5001/swagger

Header ile çalışam basic api-key authorization yapısı mevcut, swagger testleri için ilgili keye appsettings.development.json dan ulaşılabilir.

### API Endpointleri

Projenin ana API'leri aşağıda belirtilmiştir:

- **GET /api/products**: Mevcut ürünleri ve fiyatlarını listeler. Data kaynağı olarak Balance Management API kullanılır
- **POST /api/orders/create**: Yeni bir sipariş oluşturur ve ödeme rezervasyonu yapar. Kullanıcının balance kontrolü yapılır => Ürünler çekilir stok ve tutar kontrolleri yapılır => Balance Management API üzerinden PreOrder işlemi yapılır => Db'ye Order yazılır ve response dönülür => Başarısız senaryoda ilgili rollback çalışır (DB update ve Balance Management API - Cancel )
- **POST /api/orders/{id}/complete**: Siparişi tamamlar ve ödemeyi gerçekleştirir. Balance Management API - complete akışı işletilir ve Db'de Order status güncellenir

### Testler

Projede birim testleri bulunmaktadır. Testleri çalıştırmak için aşağıdaki komutları kullanabilirsiniz.

#### Birim Testlerini Çalıştırma
```bash
dotnet test
```

## Mimari

Proje, **Clean Architecture** prensiplerine uygun olarak yapılandırılmıştır:

- **Domain Layer**: İş mantığı ve domain nesneleri.
- **Application Layer**: Servisler ve iş süreçleri.
- **Infrastructure Layer**: API entegrasyonları, veri erişim katmanı.
- **API Layer**: Web API katmanı.
