// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable LocalizableElement
// ReSharper disable StringLiteralTypo

/*
 * Простая запускалка для АРМ Каталогизатор.
 * Занимается тем, что подсовывает Каталогизатору
 * копию INI-файла, не позволяя ему портить его.
 *
 * Рассчитана на .NET 2.0 и выше.
 */

#region Using declarations

using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

#endregion

namespace IrbisRunner
{
    internal static class Program
    {
        private static void ShowMessage
            (
                string message
            )
        {
            MessageBox.Show
                (
                    message,
                    "ИРБИС",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
        }

        [STAThread]
        private static void Main
            (
                string[] args
            )
        {
            if (args.Length != 2)
            {
                ShowMessage
                    (
                        "Параметры запуска:\r\n\r\n"
                        + "1. Приложение\r\n"
                        + "2. INI-файл"
                    );
                return;
            }

            try
            {
                string exeFile = args[0];
                string iniFile = args[1];

                if (!File.Exists(exeFile))
                {
                    ShowMessage
                        (
                            "Нет EXE-файла: " + exeFile
                        );
                    return;
                }

                exeFile = Path.GetFullPath(exeFile);
                string exeFolder = Path.GetDirectoryName(exeFile);
                if (string.IsNullOrEmpty(exeFolder))
                {
                    ShowMessage("Проблема с получением пути к EXE-файлу");
                    return;
                }

                if (!File.Exists(iniFile))
                {
                    ShowMessage
                        (
                            "Нет INI-файла: " + iniFile
                        );
                    return;
                }

                if (!Directory.Exists("Temp"))
                {
                    Directory.CreateDirectory("Temp");

                    if (!Directory.Exists("Temp"))
                    {
                        ShowMessage("Проблема с созданием временной папки");
                        return;
                    }
                }
                
                iniFile = Path.GetFullPath(iniFile);

                string tempFile = Path.Combine
                    (
                        exeFolder,
                        (
                          "Temp\\"
                          + Path.GetFileNameWithoutExtension(iniFile) 
                          + Guid.NewGuid().ToString("N")
                          + ".ini"
                        )
                    );

                File.Copy
                    (
                        iniFile,
                        tempFile
                    );

                if (!File.Exists(tempFile))
                {
                    ShowMessage
                        (
                            "Проблема с созданием временного INI-файла"
                        );
                    return;
                }

                ProcessStartInfo startInfo = new ProcessStartInfo
                    (
                        exeFile,
                        "Temp\\" + Path.GetFileName(tempFile)
                    )
                {
                    UseShellExecute = false,
                    WorkingDirectory = exeFolder
                };

                Process process = Process.Start(startInfo);
                process.WaitForExit();

                File.Delete(tempFile);
            }
            catch (Exception ex)
            {
                ShowMessage(ex.ToString());   
            }
        }
    }
}
