// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* Program.cs -- program entry point.
 * Ars Magna project, http://arsmagna.ru
 */

#region Using directives

using System;
using System.IO;

#endregion

// ReSharper disable CommentTypo
// ReSharper disable InconsistentNaming
// ReSharper disable ClassNeverInstantiated.Global

/*
 * Простая "повреждалка" для файлов.
 * Имитирует повреждение файла в результате
 * сбоя жёсткого диска или "сумасшествия"
 * программы: записывает куски мусора
 * произвольной длины в произвольные места 
 * файла.
 *
 * Запуск: corruptioner.exe <filename>
 *
 * Выводит в stdout краткий отчёт о записанных
 * кусках мусора.
 */

namespace Corruptioner
{
    internal class Program
    {
        private static readonly Random rand = new Random();
        private static string fileName;
        private static long fileSize;
        private static int damageCounter;
        
        public static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: corruptioner.exe <filename>");
                return;
            }

            fileName = args[0];
            
            try
            {
                fileSize = new FileInfo(fileName).Length;
                
                /* Сколько повреждений собираемся нанести */
                var maxDamage = Math.Min (100, (int) (fileSize / (64 * 1024) + 2));
                damageCounter = rand.Next(2, maxDamage);
                
                using (var stream = File.OpenWrite(fileName))
                {
                    for (var i = 0; i < damageCounter; i++)
                    {
                        var damageSize = rand.Next(2, 4097); // Размер повреждения
                        long damagePoint = rand.Next((int) fileSize); // Смещение
                        var garbage = new byte[damageSize]; // Мусор
                        rand.NextBytes(garbage);
                        
                        Console.Write
                            (
                                "{0} => offset {1:X8}, size {2:X8} ... ",
                                i,
                                damagePoint,
                                damageSize
                            );
                        
                        stream.Seek(damagePoint, SeekOrigin.Begin);
                        stream.Write(garbage, 0, damageSize);
                        
                        Console.WriteLine("done");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error: " + ex.Message);
                Environment.Exit(1);
            }
        }
    }
}
