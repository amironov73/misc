// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// ReSharper disable CheckNamespace

#region Using directives

using System.Diagnostics;
using System.Net;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Polly;
using Polly.Retry;

#endregion

namespace KrayLib;

/// <summary>
/// Приложение.
/// </summary>
public sealed class KrayApplication
{
    private readonly bool _ansi;
    private readonly string _inputFileName;
    private readonly string _outputFileName;
    private readonly bool _verbose;
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;
    private readonly ResiliencePipeline _resilience;
    private readonly int? _limit;
    private readonly bool _dryRun;
    private readonly bool _standardInput;
    private readonly string? _prompt;
    private readonly bool _listModels;

    public KrayApplication
        (
            AppParameters parameters,
            IHost host
        )
    {
        var services = host.Services;
        _configuration = services.GetRequiredService<IConfiguration>();
        _logger = services.GetRequiredService<ILogger<KrayApplication>>();
        _resilience = CreateResiliencePipeline();
        _inputFileName = parameters.InputFileName;
        _outputFileName = parameters.OutputFileName;
        _verbose = parameters.Verbose;
        _ansi = parameters.AnsiInput;
        _limit = parameters.Limit;
        _dryRun = parameters.DryRun;
        _standardInput = parameters.StandardInput;
        _prompt = parameters.Prompt;
        _listModels = parameters.ListModels;

        if (!File.Exists (_inputFileName))
        {
            throw new FileNotFoundException ("Input file not found", _inputFileName);
        }
    }

    /// <summary>
    /// Создание "переговорщика".
    /// </summary>
    public LlamaTalker CreateTalker() =>
        new (_logger, _configuration, _resilience);

    public void ListModels()
    {
        using var reader = OpenInput();
        using var writer = OpenOutput();

        try
        {
            if (InputPackage.Parse (reader) is { } input)
            {
                var talker = CreateTalker();
                var models = talker.ListOllamaModels (input);
                var output = new OutputPackage
                {
                    Message = string.Join (Environment.NewLine, models)
                };
                talker.WriteOutput (writer, output, input);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError (ex, "An error occurred: {Message}", ex.Message);
            Console.WriteLine (ex);
            throw;
        }
    }

    public void Run()
    {
        _logger.LogInformation ("Starting application");
        _logger.LogInformation ("Input file: {InputFileName}", _inputFileName);
        _logger.LogInformation ("Output file: {OutputFileName}", _outputFileName);

        if (_listModels)
        {
            ListModels();
            return;
        }

        var startupMoment = Stopwatch.GetTimestamp();
        var talker = CreateTalker();

        using var reader = OpenInput();
        using var writer = OpenOutput();

        try
        {
            var serialNumber = 1;
            while (InputPackage.Parse (reader) is { } input)
            {
                input.SerialNumber = serialNumber;
                var output = _dryRun
                    ? new OutputPackage { Message = "Some message from the LLM" }
                    : talker.Call (input);
                talker.WriteOutput (writer, output, input);
                TellWhatWeDoing (input, output);

                serialNumber++;
                if (serialNumber >= _limit)
                {
                    _logger.LogInformation ("Limit reached: {Limit}", _limit.Value);
                    if (_verbose)
                    {
                        Console.WriteLine ($"Limit reached: {_limit}");
                    }

                    break;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError (ex, "An error occurred: {Message}", ex.Message);
            // Console.WriteLine (ex);
            throw;
        }

        var timeSpent = Stopwatch.GetElapsedTime (startupMoment)
            .ToString();
        _logger.LogInformation ("Application stopped successfully");
        _logger.LogInformation ("Time spent: {Spent}", timeSpent);
        if (_verbose)
        {
            Console.WriteLine ($"Time spent: {timeSpent}");
        }
    }

    private TextReader OpenInput()
    {
        if (_standardInput)
        {
            return Console.In;
        }

        if (!string.IsNullOrEmpty (_prompt))
        {
            var prompt = _prompt;
            if (!prompt.Contains ("*****"))
            {
                prompt += (Environment.NewLine + "*****");
            }

            return new StringReader (prompt);
        }

        return InputPackage.OpenFile (_inputFileName, _ansi);
    }

    private TextWriter OpenOutput() =>
        _standardInput ? Console.Out
        : new StreamWriter (_outputFileName);

    private void TellWhatWeDoing
        (
            InputPackage input,
            OutputPackage output
        )
    {
        if (!_verbose)
        {
            return;
        }

        var usage = output.Usage!;
        Console.WriteLine ($"{input.SerialNumber}: {input.Id}: {output.Duration}");
        Console.WriteLine ($"  {usage.InputTokenCount}, {usage.OutputTokenCount}, {usage.TotalTokenCount}");
        Console.WriteLine ($"  {input.GetExceprpt()}");
        Console.WriteLine ($"  {output.GetExceprpt()}");
        Console.WriteLine();
    }

    private ResiliencePipeline CreateResiliencePipeline()
    {
        var result = new ResiliencePipelineBuilder()
            .AddRetry (new RetryStrategyOptions
            {
                BackoffType = DelayBackoffType.Constant,
                Delay = TimeSpan.FromSeconds (3),
                MaxRetryAttempts = 4,
                OnRetry = args =>
                {
                    if (args.Outcome.Exception is { } ex)
                    {
                        _logger.LogError
                            (
                                ex,
                                "OnRetry, attempt: {Attempt}, delay: {Delay}, message: {Message}",
                                args.AttemptNumber,
                                args.RetryDelay,
                                ex.Message
                            );
                    }
                    else
                    {
                        _logger.LogInformation
                            (
                                "OnRetry, attempt: {Attempt}, delay: {Delay}",
                                args.AttemptNumber,
                                args.RetryDelay
                            );
                    }

                    return default;
                }
            })
            .Build();

        return result;
    }
}
