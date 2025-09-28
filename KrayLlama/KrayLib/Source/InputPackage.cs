// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// ReSharper disable CheckNamespace

#region Using directives

using System.Globalization;
using System.Text;

using OpenAI.Chat;

#endregion

namespace KrayLib;

/// <summary>
/// Сведения, необходимы для отсылки запроса.
/// </summary>
public sealed class InputPackage
{
    #region Properties

    /// <summary>
    /// Порядковый номер пакета.
    /// </summary>
    public int SerialNumber { get; set; }

    /// <summary>
    /// Произвольный индикатор, позволяющий отличить одну запись от другой.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Точка подключения к API.
    /// </summary>
    public string? Endpoint { get; set; }

    /// <summary>
    /// Идентификатор модели.
    /// </summary>
    public string? ModelId { get; set; }

    /// <summary>
    /// Ключ для доступа к API (необязателен для Ollama).
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Картинки для анализа.
    /// </summary>
    public List<string>? Images { get; set; }

    /// <summary>
    /// Собранный из строк промпт.
    /// </summary>
    public List<string> Prompt { get; } = new ();

    /// <summary>
    /// Температура (опционально).
    /// </summary>
    public float? Temperature { get; set; }

    /// <summary>
    /// Опционально.
    /// </summary>
    public int? TopK { get; set; }

    /// <summary>
    /// Опционально.
    /// </summary>
    public float? TopP { get; set; }

    /// <summary>
    /// Опционально.
    /// </summary>
    public long? Seed { get; set; }

    /// <summary>
    /// Опционально.
    /// </summary>
    public int? MaxOutputTokens { get; set; }

    /// <summary>
    /// Режим вывода.
    /// </summary>
    public int? OutMode { get; set; }

    /// <summary>
    /// Уровень детализации изображения.
    /// </summary>
    public ChatImageDetailLevel? DetailLevel { get; set; }

    #endregion

    #region Private members

    private static float? ParseFloat (string? value) =>
        string.IsNullOrWhiteSpace (value)
            ? null
            : float.Parse
                (
                    value,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture
                );

    private static string? ParseString (string? value) =>
        string.IsNullOrWhiteSpace (value)
            ? null
            : value.Trim();

    private static long? ParseInt64 (string? value) =>
        string.IsNullOrWhiteSpace (value)
            ? null
            : long.Parse
                (
                    value,
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture
                );

    /// <summary>
    /// Разбор натурального (целого положительного) числа.
    /// </summary>
    private static int? ParseNatural (string? text) =>
        string.IsNullOrWhiteSpace (text)
            ? null
            : int.Parse
                (
                    text,
                    NumberStyles.AllowLeadingWhite
                    | NumberStyles.AllowTrailingWhite,
                    CultureInfo.InvariantCulture
                );

    #endregion

    #region Public methods

    /// <summary>
    /// Краткое изложение промпта.
    /// </summary>
    public string GetExceprpt()
        => string.Join (" ", Prompt).Substring (0, 70);

    /// <summary>
    /// Открытие файла на чтение.
    /// </summary>
    public static TextReader OpenFile
        (
            string fileName,
            bool ansi
        )
    {
        TextReader result;

        if (ansi)
        {
            Encoding.RegisterProvider (CodePagesEncodingProvider.Instance);

            var cp1251 = Encoding.GetEncoding(1251);
            result = new StreamReader (fileName, cp1251);
        }
        else
        {
            result = new StreamReader (fileName); // UTF-8
        }

        return result;
    }

    /// <summary>
    /// Получение очередной порции входных данных.
    /// </summary>
    public static InputPackage? Parse
        (
            TextReader reader
        )
    {
        var result = new InputPackage();

        var haveData = false;
        var complete = false;
        while (!complete)
        {
            var line = reader.ReadLine();
            if (line is null)
            {
                break;
            }

            line = line.Trim();
            if (string.IsNullOrEmpty (line))
            {
                continue;
            }

            haveData = true;
            if (line == "*****")
            {
                complete = true;
                continue;
            }

            if (line.Contains ('='))
            {
                var parts = line.Split ('=', 2, StringSplitOptions.TrimEntries);
                if (parts.Length != 2)
                {
                    result.Prompt.Add (line);
                    continue;
                }

                var key = parts[0];
                var value = parts[1];

                try
                {
                    switch (key)
                    {
                        case "WORKDIR":
                        case "GUID":
                            // это не для нас, игнорируем
                            continue;

                        case "ENDPOINT":
                            result.Endpoint = ParseString (value);
                            continue;

                        case "MODEL":
                            result.ModelId = ParseString (value);
                            continue;

                        case "ID":
                            result.Id = ParseString (value);
                            continue;

                        case "APIKEY":
                            result.ApiKey = ParseString (value);
                            continue;

                        case "TEMPERATURE":
                            result.Temperature = ParseFloat (value);
                            continue;

                        case "TOP_K":
                            result.TopK = ParseNatural (value);
                            continue;

                        case "TOP_P":
                            result.TopP = ParseFloat (value);
                            continue;

                        case "SEED":
                            result.Seed = ParseInt64 (value);
                            continue;

                        case "OUTMODE":
                            result.OutMode = ParseNatural (value);
                            continue;

                        case "MAX_OUTPUT_TOKENS":
                            result.MaxOutputTokens = ParseNatural (value);
                            continue;

                        case "IMAGE":
                            result.Images ??= new ();
                            result.Images.Add (value);
                            continue;

                        case "AUDIO":
                            continue;

                        case "VIDEO":
                            continue;

                        case "DOCUMENT":
                            continue;

                        case "IMAGEDETAILLEVEL":
                            result.DetailLevel = value switch
                            {
                                "low" => ChatImageDetailLevel.Low,
                                "high" => ChatImageDetailLevel.High,
                                "auto" => ChatImageDetailLevel.Auto,
                                _ => null
                            };
                            continue;

                        default:
                            result.Prompt.Add (line);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidConfigurationException
                        (
                            "Unexpected value at line: " + line,
                            ex
                        );
                }
            }
            else
            {
                result.Prompt.Add (line);
            }
        }

        if (!haveData)
        {
            return null;
        }

        if (!complete)
        {
            throw new InvalidConfigurationException ("Unexpected end of file");
        }

        if (result.Prompt.Count == 0)
        {
            throw new InvalidConfigurationException ("No prompt provided");
        }

        return result;
    }

    #endregion
}
