using Akavache;
using Akavache.Sqlite3;
using Akavache.SystemTextJson;
using Avalonia;
using ReactiveUI.Avalonia;
using Splat;
using System;
using System.IO;

namespace TestAkavacheWithAvaloniaReactiveUI
{
    internal sealed class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace()
                .UseReactiveUI(rxAppBuilder =>
                {
                    // 1
                    //rxAppBuilder
                    //    .WithAkavache<SystemJsonSerializer>(
                    //    "MyApp",
                    //    instance => { });

                    // 2
                    //Splat.Builder.AppBuilder.CreateSplatBuilder()
                    //    .WithAkavacheCacheDatabase<SystemJsonSerializer>(builder =>
                    //        builder.WithApplicationName("MyApp")
                    //               .WithSqliteProvider()
                    //               .WithSqliteDefaults());

                    // 3
                    //Splat.Builder.AppBuilder.CreateSplatBuilder()
                    //    .WithAkavache<SystemJsonSerializer>(
                    //        "MyApp",
                    //        builder => builder.WithSqliteProvider()
                    //                          .WithSqliteDefaults(),
                    //        (splat, instance) => splat.RegisterLazySingleton(() => instance.LocalMachine));

                    // 4
                    var cachePath = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                            "MyApp", "cache.db");

                    Directory.CreateDirectory(Path.GetDirectoryName(cachePath)!);

                    var blobCache = new SqliteBlobCache(cachePath, new SystemJsonSerializer());

                    AppLocator.RegisterLazySingleton<IBlobCache>(() => blobCache);
                });
    }
}
