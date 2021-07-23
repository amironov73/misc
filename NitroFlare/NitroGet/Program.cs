// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// ReSharper disable CommentTypo
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

/* Program.cs -- точка входа в программу
 */

#region Using directives

using System;

using NitroFlare;

#endregion

#nullable enable

namespace NitroGet
{
    /// <summary>
    /// Точка входа в программу.
    /// </summary>
    static class Program
    {
        static int Main(string[] args)
        {
            var client = NitroClient.FromJson();
            var progress = new ConsoleProgress();

            var key = client.GetKeyInfo();
            if (key is null)
            {
                Console.Error.WriteLine("Can't get key");
                return 1;
            }

            if (!key.IsActive())
            {
                Console.Error.WriteLine("Key is not active");
                return 1;
            }

            Console.WriteLine($"Today traffic left: {(key.TrafficLeft / 1024 / 1024):N0} Mb");

            if (args.Length != 0)
            {
                foreach (var url in args)
                {
                    client.DownloadFile(url, progress);
                }

                Console.WriteLine();
                Console.WriteLine("ALL DONE");
                Console.WriteLine();
            }

            return 0;
        }
    }
}
