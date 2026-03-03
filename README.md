# Test Project: Akavache 11.5.1 with ReactiveUI.Avalonia 11.4.7

This minimal project demonstrates an initialization failure of Akavache when used with the latest versions of ReactiveUI.Avalonia (11.4.7) and Akavache (11.5.1). It was created to isolate and report the issue to the Akavache/ReactiveUI maintainers.

## Problem Description

When trying to initialize Akavache using any of the officially documented builder methods (`WithAkavache` on `ReactiveUIBuilder` or `WithAkavacheCacheDatabase` on `Splat.Builder.AppBuilder`), a `System.MissingMethodException` is thrown:

```
System.MissingMethodException: 'Method not found: 'Void Splat.DependencyResolverMixins.RegisterLazySingleton(Splat.IMutableDependencyResolver, System.Func`1<System.__Canon>, System.String)'.
```

The exception originates inside Akavache's `WithSerializer<T>` method, which attempts to call an older overload of `RegisterLazySingleton` that expects a contract string parameter. This overload no longer exists in Splat 19.3.1 (which is pulled in transitively by ReactiveUI.Avalonia 11.4.7).

## Project Structure

The project is a standard Avalonia MVVM Application created from the template "Avalonia .NET MVVM App" with the following settings:
- **Target Framework:** .NET 10.0
- **ReactiveUI flavor** (instead of CommunityToolkit)
- **Compiled Bindings:** Enabled
- **Remove View Locator:** Сhecked

### Package Versions (as of 2026-03-03)
- `ReactiveUI.Avalonia` – 11.4.7
- `Akavache.Sqlite3` – 11.5.1
- `Akavache.SystemTextJson` – 11.5.1
- `Avalonia` – 11.3.12
- `Splat` (transitive) – 19.3.1

## How to Reproduce the Issue

1. Clone this repository.
2. Open the solution file `TestAkavacheWithAvaloniaReactiveUI.slnx` in your IDE (Visual Studio 2022+ / Rider).
3. Build and run the application. It will work because **no Akavache initialization code is currently active**.
4. To see the error, uncomment **any** of the three Akavache initialization blocks in `Program.cs`. They are all commented out and labeled as `Approach 1`, `Approach 2`, and `Approach 3`.
5. Rebuild and run. The application will crash at startup with the `MissingMethodException` described above.

## Working Workaround (Manual Cache Registration)

During investigation, a working alternative was found: **bypassing the Akavache builder and directly creating and registering the cache instance**. This proves that the underlying cache classes themselves are compatible with Splat 19+.

The working code (currently active in `Program.cs`) is:

```csharp
var cachePath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    "MyApp", "cache.db");

// Ensure the directory exists
Directory.CreateDirectory(Path.GetDirectoryName(cachePath)!);

var blobCache = new SqliteBlobCache(cachePath, new SystemJsonSerializer());
AppLocator.RegisterLazySingleton<IBlobCache>(() => blobCache);
```

With this approach, the application starts without exceptions, and the `IBlobCache` service can be successfully injected and used (e.g., in `MainWindowViewModel`).

## Goal of This Project

This repository serves as a minimal, self-contained reproduction for the issue, intended to be linked in a bug report to the [Akavache GitHub repository](https://github.com/reactiveui/Akavache). It demonstrates that:

1. The official Akavache builder API is currently broken with Splat 19+.
2. A simple workaround exists, confirming that the core caching functionality is intact.
3. The issue is likely in the Akavache builder extensions, not in the core classes.

---
*This project was created for diagnostic purposes and is not intended for production use.*
