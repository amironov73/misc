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

namespace KrayLib;

/// <summary>
/// Верхний уровень кода приложения.
/// </summary>
internal static class Utils
{
    /// <summary>
    /// Преобразование пути в совместимый с UNIX путём поворота слэшей.
    /// </summary>
    public static string PathToUnix (this string path) => path.Replace ('\\', '/');
}
