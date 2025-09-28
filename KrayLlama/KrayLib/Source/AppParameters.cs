// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// ReSharper disable CheckNamespace

using System.Globalization;

namespace KrayLib;

/// <summary>
/// Аргументы командной строки.
/// </summary>
public sealed class AppParameters
{
    #region Properties

    /// <summary>
    /// Имя входного файла.
    /// </summary>
    public string InputFileName { get; }

    /// <summary>
    /// Имя выходного файла.
    /// </summary>
    public string OutputFileName { get; }

    /// <summary>
    /// Многословно?
    /// </summary>
    public bool Verbose { get; }

    /// <summary>
    /// Входной файл в кодировке ANSI CP 1251.
    /// </summary>
    public bool AnsiInput { get; }

    /// <summary>
    /// Ограничение на количество обрабатываемых входных пакетов.
    /// </summary>
    public int? Limit { get; }

    /// <summary>
    /// Сухой запуск, без обращения к LLM.
    /// </summary>
    public bool DryRun { get; }

    /// <summary>
    /// Использовать стандартные входной и выходной потоки.
    /// </summary>
    public bool StandardInput { get; }

    /// <summary>
    /// Промпт из командной строки.
    /// </summary>
    public string? Prompt { get; }

    /// <summary>
    /// Перечислить модели.
    /// </summary>
    public bool ListModels { get; set; }

    #endregion

    #region Constructors

    public AppParameters
        (
            IReadOnlyList<string> args
        )
    {
        InputFileName = "llm_input.txt";
        OutputFileName = "llm_output.txt";

        var counter = 0;
        for (var index = 0; index < args.Count; index++)
        {
            if (args[index] == "--input" || args[index] == "-i")
            {
                index++;
                counter++;
                InputFileName = args[index];
            }
            else if (args[index] == "--output" || args[index] == "-o")
            {
                index++;
                counter++;
                OutputFileName = args[index];
            }
            else if (args[index] == "--verbose" || args[index] == "-v")
            {
                Verbose = true;
            }
            else if (args[index] == "--ansi" || args[index] == "-a")
            {
                AnsiInput = true;
            }
            else if (args[index] == "--dry" || args[index] == "-d")
            {
                DryRun = true;
            }
            else if (args[index] == "--standard" || args[index] == "-s")
            {
                StandardInput = true;
            }
            else if (args[index] == "--prompt" || args[index] == "-p")
            {
                index++;
                Prompt = args[index];
            }
            else if (args[index] == "--limit" || args[index] == "-l")
            {
                index++;
                Limit = int.Parse
                    (
                        args[index],
                        NumberStyles.Integer,
                        CultureInfo.InvariantCulture
                    );
            }
            else if (args[index] == "--models" || args[index] == "-m")
            {
                ListModels = true;
            }
            else
            {
                if (args[index][0] != '-')
                {
                    switch (counter)
                    {
                        case 0:
                            InputFileName = args[index];
                            counter++;
                            break;

                        case 1:
                            OutputFileName = args[index];
                            counter++;
                            break;
                    }
                }
            }
        }
    }

    #endregion

    #region Public methods

    public string DetermineWorkDirectory()
    {
        var result = Directory.GetCurrentDirectory();
        var candidate = Path.GetDirectoryName (OutputFileName);
        if (!string.IsNullOrEmpty (candidate))
        {
            result = candidate;
        }

        return result;
    }

    #endregion
}
