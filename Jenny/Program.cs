// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// ReSharper disable CheckNamespace
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable CommentTypo
// ReSharper disable LocalizableElement
// ReSharper disable StringLiteralTypo

/* Program.cs -- инициализация и точка входа в программу
 * Ars Magna project, http://arsmagna.ru
 */

#region Using directives

using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Parsing;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
 
using NLog.Extensions.Logging;

#endregion

#nullable enable

internal sealed class Program
{
    private static readonly Argument<string> InputPath = new ()
    {
        Name = "InputPath",
        Arity = ArgumentArity.ExactlyOne,
        Description = "Input path"
    };
    
    private static readonly Argument<string> OutputPath = new ()
    {
        Name = "OutputPath",
        Arity = ArgumentArity.ExactlyOne,
        Description = "Output path"
    };

    /// <summary>
    /// Точка входа в программу.
    /// </summary>
    public static async Task<int> Main 
        (
            string[] args
        )
    {
        try
        {
            var config = new ConfigurationBuilder()
                .SetBasePath (AppContext.BaseDirectory)
                .AddJsonFile ("appsettings.json", optional: false)
                .Build();

            // using var host = CreateHostBuilder (args, config);
            var rootCommand = new RootCommand ("Jenny -- little seamtress")
            {
                InputPath,
                OutputPath
            };

            rootCommand.Handler = CommandHandler.Create <IHost, JennyArgs> (Run);

            await new CommandLineBuilder(rootCommand)
                .UseDefaults()
                .UseHost (arguments => CreateHostBuilder (arguments, config))
                .Build()
                .InvokeAsync (args);
        }
        catch (Exception exception)
        {
            await Console.Error.WriteLineAsync (exception.ToString());

            return 1;
        }

        // к этому моменту все успешно выполнено либо произошла ошибка
        await Console.Out.WriteLineAsync ("THAT'S ALL, FOLKS!");
        
        return 0;
    }
    
    /// <summary>
    /// Создание хоста приложения.
    /// </summary>
    private static IHostBuilder CreateHostBuilder
        (
            string[] args, 
            IConfigurationRoot config
        )
    {
        return Host.CreateDefaultBuilder (args)
            .ConfigureServices (configureDelegate: services =>
            {
                services.AddOptions();

                // включаем логирование
                services.AddLogging (logging =>
                {
                    logging.ClearProviders();
                    logging.AddNLog (config);
                });

                // регистрируем интерфейсы
                // services.AddTransient<IGreeter, InternationalGreeter>();

                // откуда брать настройки
                // var jennyArgs = new JennyArgs
                //     (
                //         parseResult.GetValueForArgument (_inputPath),
                //         parseResult.GetValueForArgument (_outputPath)
                //     );
                // services.AddSingleton (jennyArgs);

                // регистрируем наш хост-сервис
                // services.AddHostedService<JennyActivity>();
            });
    }

    private static async Task<int> Run
        (
            IHost host,
            JennyArgs args
        )
    {
        var logger = host.Services.GetRequiredService<ILogger<Program>>();

        int result;
        try
        {
            logger.LogInformation ("Starting");
            
            result = await new Seamstress (host, args).Run();
            
            logger.LogInformation ("Stopping");
        }
        catch (Exception exception)
        {
            logger.LogError (exception, "Exception occurred");
            
            return 1;
        }
        
        return result;
    }
}
