// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// ReSharper disable CheckNamespace
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo

/* Program.cs -- точка входа в приложение
 * Ars Magna project, http://arsmagna.ru
 */

#region Using directives

using System;

using Avalonia;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

#endregion

namespace FastInventoryBook;

internal sealed class Program
{
    public static IConfiguration Configuration { get; private set; } = null!;

    public static IHost ApplicationHost { get; private set; } = null!;

    [STAThread]
    public static void Main
        (
            string[] args
        )
    {
        Initialize (args);

        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime (args);
    }

    private static void Initialize
       (
            string[] args
       )
    {
        var builder = Host.CreateDefaultBuilder (args);
        Configuration = new ConfigurationBuilder()
            .SetBasePath (AppContext.BaseDirectory)
            .AddJsonFile ("appsettings.json", true, true)
            .AddEnvironmentVariables()
            .AddCommandLine (args)
            .Build();


        ApplicationHost = builder.Build();
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    private static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
    }
}
