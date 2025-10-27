// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// ReSharper disable CheckNamespace

#region Using directives

using KrayLib;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using ZLogger;

#endregion

/// <summary>
/// Верхний уровень кода приложения.
/// </summary>
internal static class Program
{
    /// <summary>
    /// Точка входа в приложение.
    /// </summary>
    public static int Main
        (
            string[] args
        )
    {
        var parameters = new AppParameters (args);
        var outputDirectory = parameters.DetermineWorkDirectory();

        var settings = new HostApplicationBuilderSettings
        {
            Args = args,
            ApplicationName = "KrayGemma",
            ContentRootPath = Directory.GetCurrentDirectory(),
        };
        var builder = Host.CreateEmptyApplicationBuilder (settings);

        builder.Configuration
            .AddJsonFile ("appsettings.json", optional: false);

        builder.Services.AddLogging
            (
                config =>
                {
                    config.ClearProviders();
                    config.SetMinimumLevel (LogLevel.Debug);

                    var logFileName = Path.Combine (outputDirectory, "llm_logs.txt");
                    config.AddZLoggerFile (logFileName);
                }
            );

        var host = builder.Build();

        try
        {
            var app = new KrayApplication (parameters, host);
            app.Run();
        }
        catch (Exception ex)
        {
            Console.WriteLine (ex);

            // Делаем принудительный сброс буферов на диск
            var factory = host.Services.GetRequiredService<ILoggerFactory>();
            factory.Dispose();
            Thread.Sleep (3000);
            return 1;
        }

        return 0;
    }
}
