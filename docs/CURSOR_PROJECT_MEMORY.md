# Proje belleği (Cursor / ajan)

Bu dosya **Azoxia Core** deposu için oturumlar arası bağlam sağlar. Sohbet yerine burayı güncel tutun.

## Teknoloji yığını

- **.NET**: `net10.0` (C#), `ImplicitUsings` + `Nullable` etkin (`Core`, `Application`, `Infrastructure`, `Persistence`, `Api`).
- **Roslyn analizör / kaynak üretici**: `src/Generators` → `netstandard2.0`, `Microsoft.CodeAnalysis.*` 5.3.0.
- **API yüzeyi**: ASP.NET Core (`Microsoft.AspNetCore.OpenApi` 10.0.7); `Api` projesi **çalıştırılabilir exe değil** (`OutputType` = Library) — barındırıcı uygulama `Startup` ve olay kancalarıyla (`OnConfigureServices` / `OnConfigurePipelines`) genişletir.
- **Veri**: Entity Framework Core 10.0.7 (`Relational`, `Proxies`).
- **Çözüm dosyası**: Depo kökünde `.sln` yok; derleme örneği: `dotnet build src/Api/Api.csproj`.

## Modül / proje haritası

| Proje | Konum | Bağımlılıklar (özet) | Rol |
|--------|--------|----------------------|-----|
| **Core** | `src/Core` | — | `Domain`, `IRequest` / handler sözleşmeleri, `ISender`, pipeline arabirimleri, `IServiceRegister`, `IConfig`, `IRepository` / `IUnitOfWork`, ortak uzantılar ve `ApiResponse` vb. |
| **Application** | `src/Application` | → Core | `Sender`, doğrulama pipeline (`ValidationPipelineBehavior`), komut/sorgu taban sınıfları, `IModel`, `ITokenService`, **`ICacheService`** + `CacheKey` / `CacheDependency` / `CacheEntryOptions`; `DependencyInjection/ServiceRegister`. |
| **Infrastructure** | `src/Infrastructure` | → Application | `ITokenService` (`TokenService`), JWT (`JwtConfig`), Redis (`RedisConfig`, …), **`CacheService`**, **`FileLoggingConfig`** + `FileLoggerProvider` (`DependencyInjection/ServiceRegister` içinde `AddLogging` ile kayıt); `DependencyInjection/ServiceRegister`. |
| **Persistence** | `src/Persistence` | → Core | `DbContextBase`, `Repository` / async varyantlar, `UnitOfWork`, EF eşlemeleri, audit interceptor; **`AppLog`** / `AppLogs` + **`EfLoggingConfig`** + `DependencyInjection.ServiceRegister.AddAzoxiaEfLogging<TDbContext>(ILoggingBuilder, IConfiguration)` (`EfLoggerProvider`, `IDbContextFactory<TDbContext>`); `DependencyInjection/ServiceRegister`. |
| **Api** | `src/Api` | → Application, Infrastructure, Persistence; Generators **Analyzer** | `Startup`, `ApiControllerBase`, `HttpExecutionContext`; HTTP pipeline ve controller kaydı. |
| **Generators** | `src/Generators` | — | `DependencyInjectionGenerator`: derlenen assembly için `IServiceRegister` ve `IConfig` uygulayıcılarını tarayıp `GeneratedServiceRegistrar.AddAzoxiaCore` üretmeyi hedefler (`{AssemblyName}.DependencyInjection` ad alanı). |

## Katman kuralları (referans bağımlılığı)

- **Application** yalnızca **Core**’a referans verir (`Infrastructure` / `Persistence` / `Api` referansı eklenmemeli).
- **Infrastructure** → Application (uygulama arabirimlerini doldurur).
- **Persistence** → Core (domain + persistence sözleşmeleri).
- **Api**, uygulama ve altyapı projelerini bir araya getirir; yeni HTTP veya host davranışı genelde `src/Api` veya tüketen host’ta genişletilir.

## Kalıplar

- **DI**: `IServiceRegister.Register(IServiceCollection, IConfiguration)`; yeni modül servisleri için implementasyon + ilgili `*DependencyInjection/ServiceRegister.cs` desenine uyum. `DependencyInjectionGenerator` üretilen `AddAzoxiaCore` her iki argümanı da geçirir.
- **Önbellek**: `ICacheService` — okuma önce L1 sonra L2; yazma L1 (kısa TTL) sonra isteğe bağlı L2 (`CacheEntryOptions.DistributedTtl`). Aggregate ör. `Country` + alt `City` için `SetAsync`’te `Dependencies` ile `City` kimliklerini ekleyin; `City` güncellemesinden sonra **`InvalidateByDependencyAsync(new CacheDependency("City", id))`** çağrısı zorunlu. Çoklu pod: L2 Redis ile tutarlılık; L1 yalnız süreç içi — tam cluster L1 temizliği için ileride `IRedisMessageBus` yayını düşünülebilir. Redis bağımlılık indeksi anahtar öneki: `azoxia:cache:dep:` (Redis SET).
- **Uygulama mesajları**: `IRequest` / `ICommand` / `IQuery` ve handler tabanları (`CommandHandlerBase`, `QueryHandlerBase`, …).
- **Denetim / EF**: `AuditableEntityBase`, `DbContextBase`, `EntityTypeConfigurationBase`, `IEntityTypeConfiguration`.
- **Kod stili**: Bu depo altında C# gövde formatı için `.cursor/skills/code-format/SKILL.md` kullanılabilir.

## Dosya ve veritabanı loglama (üçüncü parti yok)

- **Dosya**: `Azoxia.Core.Infrastructure.DependencyInjection.ServiceRegister.Register` içinde `services.AddLogging(...)` ile `FileLoggerProvider` + `FileLoggingConfig` bağlanır; `AddAzoxiaCore` çağrıldığında dosya sink devreye girer (`Enabled` kapalıysa sağlayıcı no-op). Günlük dosya: `{FileNamePrefix}-yyyyMMdd.log`, UTF-8 tek satır JSON. Özel ad alanı: `Azoxia.Core.Infrastructure.FileLogging`.
- **Veritabanı**: `Azoxia.Core.Persistence.DependencyInjection.ServiceRegister.AddAzoxiaEfLogging<TDbContext>(builder.Logging, configuration)` — barındırıcı **bir kez** çağırmalıdır (`EfLoggingConfig`, `AppLog` → `AppLogs`). **Zorunlu**: önce `AddDbContextFactory<TDbContext>(…)`. `Register` yalnızca `EfLoggingConfig` önbelleğini hazırlar. EF iç log geri beslemesi: `Microsoft.EntityFrameworkCore` / `Microsoft.Data.*` yazılmaz.
- **Yapılandırma**: `Config.GetOrCreateConfig` ile `IConfig` bölüm adları **`FileLoggingConfig`** ve **`EfLoggingConfig`** (tür adıyla aynı).

### Barındırıcı örneği (`WebApplicationBuilder`)

Aşağıdaki çağrılar `WebApplication.CreateBuilder` sonrası, `Build()` öncesinde (ör. `Startup.OnConfigureServices` aboneliğinde veya exe `Program.cs` içinde) tipiktir:

```csharp
// Somut DbContext (ör. host projesinde): AppLog eşlemesi Persistence derlemesinden ApplyConfigurationsFromAssembly ile gelir.
builder.Services.AddDbContextFactory<MyAppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"))); // örnek sağlayıcı

// Dosya logu: Azoxia.Core.Infrastructure.DependencyInjection.ServiceRegister → AddAzoxiaCore ile eklenir.
Azoxia.Core.Persistence.DependencyInjection.ServiceRegister.AddAzoxiaEfLogging<MyAppDbContext>(builder.Logging, builder.Configuration);
```

Örnek `appsettings` parçası:

```json
"FileLoggingConfig": {
  "Enabled": true,
  "LogDirectory": "logs",
  "FileNamePrefix": "app",
  "MinimumLevel": "Information",
  "RetainedFileCountLimit": 7
},
"EfLoggingConfig": {
  "Enabled": true,
  "MinimumLevel": "Warning",
  "BatchSize": 50,
  "CoalesceMilliseconds": 50
}
```

Process kapanırken dosya tamponunu boşaltmak için `FileLoggerProvider.Dispose` kanalı tamamlar; uzun süren flush için barındırıcıda `IHostApplicationLifetime` ile ek bekleme gerekmez.

## Test

- Şu an ayrı `*Tests*.csproj` yok; test projesi eklendiğinde bu bölümü güncelleyin.

## Mimari kararlar

- *(İlk resmi kilidi buraya ekleyin: örn. “Unit of Work tek transaction politikası”, “JWT claim şeması”.)*

## Son önemli değişiklikler

- **2026-05**: Redis — `RedisConfig` (`IConfig`, bölüm adı `RedisConfig`), `Enabled` açıkken `AddStackExchangeRedisCache`, `IConnectionMultiplexer`, `IRedisMessageBus` (`RedisMessageBus` / kapalıyken `NullRedisMessageBus`). `Enabled` kapalıyken `AddDistributedMemoryCache` ile `IDistributedCache` (process içi L2).
- **2026-05**: `ICacheService` / `CacheService` — iki katman + JSON L2 + bağımlılık invalidation; barındırıcıda `AddMemoryCache` gerekir (`Startup` mevcut).
- **2026-05**: Dosya logu (`Infrastructure.ServiceRegister` + `FileLoggingConfig`) ve EF logu (`Persistence.ServiceRegister.AddAzoxiaEfLogging<T>`, `EfLoggingConfig`, `AppLog` / `AppLogs`, `IDbContextFactory<TDbContext>`).

## Bilinçli sınırlar

- Katman referanslarını tersine çevirme (ör. Core → Persistence).
- `Generators` mantığını değiştirirken tüm tüketen assembly’lerde üretilen `AddAzoxiaCore` imzasını ve bulunabilirliğini doğrulayın.

---

**Bakım**: Önemli özellik veya yapı değişikliğinden sonra bu dosyayı kısa maddelerle güncelleyin.
