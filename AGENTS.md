# Azoxia Core — Agent rehberi

**Azoxia Core**: Katmanlı .NET 10 (`net10.0`) kitaplıkları + `Api` (ASP.NET Core **library**). Ayrıntılı harita ve kurallar: `docs/CURSOR_PROJECT_MEMORY.md`.

## Hızlı kurallar

1. İşe **analiz** ile başla; çok dosya veya mimari etki varsa iş listesi kullan.
2. **Referans yönü**: Application → yalnız Core; Infrastructure → Application; Persistence → Core; Api hepsini birleştirir (`CURSOR_PROJECT_MEMORY.md` tablosu).
3. Kalıcı bilgiyi `docs/CURSOR_PROJECT_MEMORY.md` ile senkron tut.

## Derleme

- Kökte `.sln` yok: `dotnet build src/Api/Api.csproj` (veya değiştirdiğin projenin `.csproj` yolu).

## Teknik notlar

- Paket sürümleri mevcut `PackageReference` ile hizalı kalsın (EF 10.0.7, Extensions 10.0.7, vb.).
- **UI / mobil**: Bu depo backend çekirdeğidir; responsive / dp-sp kuralları istemci (MAUI, RN, web) repolarında geçerlidir.

## Önerilen ilk adımlar

1. `docs/CURSOR_PROJECT_MEMORY.md` oku.
2. Dokunacağın katmandaki komşu dosyaları ve mevcut `ServiceRegister` / handler desenlerini incele.
3. Analiz → (gerekirse) iş listesi → uygulama → özet ve bellek güncellemesi.
