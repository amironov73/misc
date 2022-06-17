// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// ReSharper disable CheckNamespace
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable CommentTypo
// ReSharper disable LocalizableElement
// ReSharper disable StringLiteralTypo

/* JennyArgs.cs -- конфигурация для швеи
 * Ars Magna project, http://arsmagna.ru
 */

#region Using directives

using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

#endregion

#nullable enable

/// <summary>
/// Конфигурация для швеи.
/// </summary>
internal sealed class JennyArgs
{
    #region Properties

    /// <summary>
    /// Имя директории с исходными файлами.
    /// </summary>
    public string InputPath { get; }
    
    /// <summary>
    /// Имя выходного PDF-файла.
    /// </summary>
    public string OutputPath { get; }

    #endregion

    #region Construction

    /// <summary>
    /// Конструктор.
    /// </summary>
    public JennyArgs
        (
            string inputPath, 
            string outputPath
        )
    {
        InputPath = inputPath;
        OutputPath = outputPath;
    }

    #endregion
}
