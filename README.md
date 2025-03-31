# EventManagementApi (English)

EventManagementApi is a web API project built using **ASP.NET Core (.NET 8)**, designed to manage events and provide a **multi-tenant architecture**.

## Project Structure

The project is organized into the following directories:

- **EventManagementApi**: The main project directory containing the API code.
- **Infrastructure**: Contains infrastructure-related code, including data access, middleware, and mapping configurations.
  - **Caching**: Contains caching-related code, including the `RedisCacheService`.
  - **Data**: Contains database migration scripts and data access code.
  - **Helpers**: Contains helper classes, including the `JwtHelper`.
  - **Middleware**: Contains custom middleware classes, including the `TenantMiddleware` and `ExceptionHandlingMiddleware`.
  - **Mapping**: Contains AutoMapper configurations for mapping between domain models and DTOs.
  - **Persistence**: Contains the `EventManagementDbContext` class, which defines the database context.
  - **Services**: Contains service classes, including the `UserContextProvider`.
- **Domain**: Contains the domain logic and models for the application.
- **MultiTenancy**: Contains code related to multi-tenancy, including the `TenantProvider` class.
- **Api**: Contains the API controllers and related code.
  - **Controllers**: Contains the API controllers for managing events, tenants, and other resources.
- **Shared**: Contains shared code and constants.
  - **Constants**: Contains constant values, including enums for roles and other application-specific constants.
- **Tests**: Contains unit tests and integration tests for the application.
  - **AuthenticationServiceTests**: Tests for the authentication service.
  - **EventServiceTests**: Tests for the event service.
  - **TenantMiddlewareTests**: Tests for custom middleware components.

## Configuration

The project uses an `appsettings.json` file to store configuration settings. The file contains settings for:

- Database connection (`ConnectionStrings` section)
- JWT authentication (`JwtSettings` section)
- Multi-tenancy configuration (shared database, shared schema)
- **Redis cache settings**
- Logging configuration (if applicable)

## Features

- **Multi-tenancy**: The application supports multiple tenants, each with their own separate data and configuration.
- **Event management**: APIs for managing events, including creation, update, and deletion.
- **Authentication & Authorization**: Uses **JWT authentication** and role-based access control (RBAC).
- **Global Exception Handling**: Custom middleware to handle and log application errors.
- **Caching**: **Redis caching support** for improved performance.
- **Swagger API Documentation**: Interactive API documentation via Swagger.

## Technologies Used

- **ASP.NET Core (.NET 8)**
- **Entity Framework Core** (EF Core)
- **AutoMapper**
- **JWT Authentication**
- **Multi-tenancy Architecture**
- **Redis Caching**

## Getting Started

### Prerequisites

- Install **.NET 8 SDK**
- Install **Redis** (for caching)
  - Download Redis from: [GitHub Redis Archive](https://github.com/microsoftarchive/redis/releases)
  - After installation, start Redis and check the connection with:
    ```bash
    redis-cli ping
    ```
    If Redis is running correctly, you should see:
    ```
    PONG
    ```

### Installation & Running the Project

1. **Clone the repository**:
   ```bash
   git clone https://github.com/username/EventManagementApi.git
   cd EventManagementApi

2. **Install dependencies**:
   ```bash
   dotnet restore
   ```

3. **Configure the application**:
   - Open the `appsettings.json` file from the `EventManagementApi` directory.
   - Update the `ConnectionStrings` section with the appropriate database connection string for your environment.
   - Update the `JwtSettings` section with the appropriate JWT settings for your environment.
   - Configure the `Redis` section with the appropriate Redis settings for your environment.

4. **Apply Database migrations**:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

5. **Run the application**:
   ```bash
   dotnet run
   ```
6. **Access Swagger UI**:
   -  Open your browser and navigate to `https://localhost:44316` or `https://localhost:7001`.
   - You should see the Swagger UI for the API documentation.
   - Use the API documentation to test the endpoints.

7. **Test the API**:
   - You can use the Swagger UI to test the API.
   - Use the API documentation to test the endpoints.

8. **Run tests**:
   ```bash
   dotnet test
   ```

This format allows users to follow step-by-step instructions on how to set up and run the project. If you would like to make any additional changes or modifications, please feel free to mention them!

    
# EventManagementApi (Türkçe)

EventManagementApi, **ASP.NET Core (.NET 8)** kullanarak oluşturulmuş bir web API projesidir ve etkinlikleri yönetmek için **çok kiracılı mimari** sunar.

## Proje Yapısı

Proje aşağıdaki dizinlere ayrılmıştır:

- **EventManagementApi**: API kodlarını içeren ana proje dizini.
- **Infrastructure**: Veri erişimi, ara yazılım ve eşleme yapılandırmaları gibi altyapı ile ilgili kodları içerir.
  - **Caching**: `RedisCacheService` dahil olmak üzere önbellekleme ile ilgili kodları içerir.
  - **Data**: Veritabanı geçiş betikleri ve veri erişim kodlarını içerir.
  - **Helpers**: `JwtHelper` dahil olmak üzere yardımcı sınıfları içerir.
  - **Middleware**: `TenantMiddleware` ve `ExceptionHandlingMiddleware` gibi özel ara yazılım sınıflarını içerir.
  - **Mapping**: Alan modelleri ile DTO'lar arasında eşleme yapmak için AutoMapper yapılandırmalarını içerir.
  - **Persistence**: Veritabanı bağlamını tanımlayan `EventManagementDbContext` sınıfını içerir.
  - **Services**: `UserContextProvider` gibi servis sınıflarını içerir.
- **Domain**: Uygulamanın alan mantığını ve modellerini içerir.
- **MultiTenancy**: Çok kiracılılık ile ilgili kodları içerir, `TenantProvider` sınıfı dahil.
- **Api**: API denetleyicilerini ve ilgili kodları içerir.
  - **Controllers**: Etkinlikler, kiracılar ve diğer kaynakları yönetmek için API denetleyicilerini içerir.
- **Shared**: Paylaşılan kod ve sabitleri içerir.
  - **Constants**: Roller ve diğer uygulama ile ilgili sabit değerleri içerir.
- **Tests**: Uygulama için birim testleri ve entegrasyon testlerini içerir.
  - **AuthenticationServiceTests**: Kimlik doğrulama servisi için testler.
  - **EventServiceTests**: Etkinlik servisi için testler.
  - **TenantMiddlewareTests**: Özel ara yazılım bileşenleri için testler.

## Yapılandırma

Proje, yapılandırma ayarlarını depolamak için bir `appsettings.json` dosyası kullanır. Dosya, aşağıdaki ayarları içerir:

- Veritabanı bağlantısı (`ConnectionStrings` bölümü)
- JWT kimlik doğrulaması (`JwtSettings` bölümü)
- Çok kiracılılık yapılandırması (ortak veritabanı, ortak şema)
- Redis önbellek ayarları
- Günlükleme yapılandırması (varsa)

## Özellikler

- **Çok Kiracılılık**: Uygulama, her biri kendi verileri ve yapılandırmasıyla ayrı kiracılar için destek sunar.
- **Etkinlik Yönetimi**: Etkinlikleri yönetmek için API'ler, etkinlik oluşturma, güncelleme ve silme dahil.
- **Kimlik Doğrulama ve Yetkilendirme**: **JWT kimlik doğrulaması** ve rol tabanlı erişim kontrolü (RBAC) kullanır.
- **Global Hata Yönetimi**: Uygulama hatalarını ele almak ve kaydetmek için özel ara yazılım.
- **Önbellekleme**: **Redis önbellekleme desteği** ile performans iyileştirmesi.
- **Swagger API Dokümantasyonu**: Etkileşimli API dokümantasyonu Swagger üzerinden sağlanır.

## Kullanılan Teknolojiler

- **ASP.NET Core (.NET 8)**
- **Entity Framework Core** (EF Core)
- **AutoMapper**
- **JWT Kimlik Doğrulaması**
- **Çok Kiracılı Mimarisi**
- **Redis Önbellekleme**

## Başlarken

### Gereksinimler

- **.NET 8 SDK**'sını kurun
- **Redis** (önbellekleme için) kurun
  - Redis'i şu adresten indirebilirsiniz: [GitHub Redis Archive](https://github.com/microsoftarchive/redis/releases)
  - Kurulumdan sonra, Redis'i başlatın ve bağlantıyı şu komutla kontrol edin:
    ```bash
    redis-cli ping
    ```
    Redis doğru şekilde çalışıyorsa, aşağıdaki cevabı almanız gerekir:
    ```
    PONG
    ```

### Projeyi Yükleme ve Çalıştırma

1. **Repoyu klonlayın**:
   ```bash
   git clone https://github.com/username/EventManagementApi.git
   cd EventManagementApi

2. **Bağımlılıkları yükleyin**:
   ```bash
   dotnet restore
   ```

3. **Uygulamayı yapılandırın**:
   - `EventManagementApi` dizininden `appsettings.json` dosyasını açın.
   - `ConnectionStrings` bölümüne uygun veritabanı bağlantı dizesini ekleyin.
   - `JwtSettings` bölümüne uygun JWT ayarlarını ekleyin.
   - `Redis` bölümüne uygun Redis ayarlarını ekleyin.

4. **Veritabanı geçişlerini uygulayın**:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

5. **Uygulamayı çalıştırın**:
   ```bash
   dotnet run
   ```
6. **Swagger UI'na erişin**:
   - Tarayıcınızda `https://localhost:44316` veya `https://localhost:7001` adresine gidin.
   - API dokümantasyonu için Swagger UI'na erişin.
   - API documentation'ını kullanarak endpoints'leri test edin.

7. **API'yi test edin**:
   - Swagger UI'na erişin.
   - API documentation'ını kullanarak endpoints'leri test edin.

8. **Test edin**:
   ```bash
   dotnet test
   ```


Yukarıdaki içerik, bir **README.md** dosyası olarak doğrudan kullanılabilir. Yardımcı olabileceğim başka bir konu varsa, lütfen belirtin!

