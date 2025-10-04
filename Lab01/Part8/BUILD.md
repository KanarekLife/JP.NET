# Build.Cake

Skrypt automatyzacji build dla projektu OtwarteDane.

## Wymagania

- .NET 9.0 SDK
- Cake.Tool (instalowany automatycznie)

## Użycie

### Linux/macOS (zalecane):
```bash
./build-dotnet.sh
```

### Linux/macOS (z Mono):
```bash
./build.sh
```

### Windows PowerShell:
```powershell
.\build.ps1
```

### Windows Command Prompt:
```cmd
build.cmd
```

## Dostępne targety

- **Default** - Uruchamia pełny pipeline (Restore → Rebuild → Test → Clean Publish Folder → Publish)
- **Clean** - Czyści foldery bin i publish
- **Restore-NuGet-Packages** - Przywraca pakiety NuGet
- **Build** - Buduje rozwiązanie
- **Rebuild** - Czyści i buduje ponownie
- **Run-Unit-Tests** - Uruchamia testy jednostkowe
- **Clean-Publish-Folder** - Czyści folder publish
- **Publish** - Publikuje aplikację jako self-contained dla Linux x64
- **Publish-Portable** - Publikuje aplikację jako portable

## Przykłady użycia

```bash
# Uruchomienie domyślnego pipeline'u
./build-dotnet.sh

# Uruchomienie konkretnego targetu
./build-dotnet.sh --target=Run-Unit-Tests

# Budowanie w trybie Debug
./build-dotnet.sh --configuration=Debug

# Tylko testy
./build-dotnet.sh --target=Run-Unit-Tests

# Tylko publikacja portable
./build-dotnet.sh --target=Publish-Portable

# Wyświetlenie wersji Cake
./build-dotnet.sh --version

# Dry run (pokazuje co zostanie wykonane bez faktycznego uruchomienia)
./build-dotnet.sh --dry-run
```

## Pipeline

1. **Restore** - Przywraca pakiety NuGet
2. **Rebuild** - Czyści i buduje projekt od nowa
3. **Test** - Uruchamia wszystkie testy jednostkowe
4. **Clean Publish Folder** - Czyści folder docelowy publikacji
5. **Publish** - Publikuje aplikację gotową do dystrybucji

## Artefakty

- **Self-contained publish**: `./publish/` - Zawiera aplikację z wszystkimi zależnościami (.NET runtime)
- **Portable publish**: `./publish/portable/` - Zawiera aplikację wymagającą zainstalowanego .NET runtime
- **Test Results**: `./TestResults/` - Zawiera wyniki testów w formacie TRX
- **Build artifacts**: `./*/bin/` - Standardowe artefakty .NET

## Różnica między skryptami

- **build-dotnet.sh** - Używa `dotnet cake` (zalecane dla nowoczesnych środowisk)
- **build.sh** - Używa Mono + .NET Framework Cake (wymaga zainstalowanego Mono)
- **build.ps1** - PowerShell dla Windows
- **build.cmd** - Command Prompt dla Windows

## Struktura plików Cake

- `build.cake` - Główny skrypt Cake z definicjami tasków
- `tools/packages.config` - Konfiguracja pakietów Cake (używana przez build.sh/build.ps1)