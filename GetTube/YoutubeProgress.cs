// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* YoutubeProgress.cs -- параметры загрузки
 * Ars Magna project, http://arsmagna.ru
 */

using System;

namespace GetTube;

internal sealed class YoutubeProgress
    : IProgress<double>, IDisposable
{
    public void Report (double progress)
    {
        Console.Write ($"\r{progress:P1}");
    }

    public void Dispose()
    {
        Console.WriteLine ("\rCompleted");
    }
}
