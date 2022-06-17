// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// ReSharper disable CheckNamespace
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable CommentTypo
// ReSharper disable LocalizableElement
// ReSharper disable StringLiteralTypo

/* Seamtress.cs -- собственно портниха
 * Ars Magna project, http://arsmagna.ru
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using DevExpress.Pdf;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

#endregion

#nullable enable

/// <summary>
/// Собственно портниха.
/// </summary>
internal sealed class Seamstress
{
    #region Construction

    /// <summary>
    /// Конструктор.
    /// </summary>
    public Seamstress
        (
            IHost host,
            JennyArgs args
        )
    {
        _logger = host.Services.GetRequiredService<ILogger<Seamstress>>();
        _args = args;
        
        _temporaryPath = Path.Combine
            (
                Path.GetTempPath(),
                Guid.NewGuid().ToString ("N")
            );
        Directory.CreateDirectory (_temporaryPath);
        _logger.LogDebug ("Temporary folder is \"{TemporaryPath}\"", _temporaryPath);
    }

    #endregion

    #region Private members

    private readonly ILogger _logger;
    private readonly JennyArgs _args;
    private readonly string _temporaryPath;

    private async Task ReportMemory()
    {
        // GC.Collect (0, GCCollectionMode.Forced, true);
        // GC.Collect();
        // GC.Collect();
        
        var allocatedBytes = GC.GetTotalMemory (true);
        _logger.LogDebug ("Memory: {AllocatedBytes}", allocatedBytes);
        
        await Task.CompletedTask;
    }

    private async Task<FileInfo[]> FindInputFiles
        (
            string inputPath
        )
    {
        var fileNames = Directory.GetFiles (inputPath, "*.jpg", SearchOption.AllDirectories);
        Array.Sort (fileNames);
        _logger.LogInformation ("Input files found: {Count}", fileNames.Length);

        var list = new List<FileInfo>();
        foreach (var fileName in fileNames)
        {
            var entry = new FileInfo (fileName);
            list.Add (entry);

            await Task.CompletedTask;
        }

        return list.ToArray();
    }

    private FileInfo[][] GroupInputFiles
        (
            IEnumerable<FileInfo> inputFiles
        )
    {
        const int maxCount = 12;
        const long maxChunkSize = 5_000_000_000L;
        
        var result = new List<FileInfo[]>();
        var chunk = new List<FileInfo>();
        var chunkSize = 0L;

        foreach (var inputFile in inputFiles)
        {
            chunkSize += inputFile.Length;
            chunk.Add (inputFile);

            if (chunk.Count >= maxCount || chunkSize >= maxChunkSize)
            {
                result.Add (chunk.ToArray());
                chunk = new List<FileInfo>();
                chunkSize = 0L;
            }
        }

        if (chunk.Count != 0)
        {
            result.Add (chunk.ToArray());
        }

        return result.ToArray();
    }

    private async Task<string> ProcessChunk
        (
            int chunkIndex,
            IEnumerable<FileInfo> inputChunk
        )
    {
        var pdfName = Path.Combine
            (
                _temporaryPath,
                chunkIndex.ToString ("00000", CultureInfo.InvariantCulture) + ".pdf"
            );

        _logger.LogDebug ("Creating \"{PdfName}\"", pdfName);

        using (var processor = new PdfDocumentProcessor())
        {
            //processor.RenderingEngine = PdfRenderingEngine.Skia;
            //processor.ImageCacheSize = 10;
            //processor.DataRecognitionCacheSize = 10;
            
            processor.CreateEmptyDocument (pdfName);

            // var document = processor.Document;
            foreach (var imageFile in inputChunk)
            {
                _logger.LogInformation
                    (
                        "Processing \"{ImageFile}\" ({FileSize} Kb)",
                        imageFile,
                        imageFile.Length / 1024
                    );
                await ReportMemory();

                using var image = new Bitmap (imageFile.FullName);
                var pageSize = new PdfRectangle
                    (
                        0,
                        0,
                        image.Width,
                        image.Height
                    );
                var rectangle = new RectangleF
                    (
                        0,
                        0,
                        (float)pageSize.Width,
                        (float)pageSize.Height
                    );

                using var graphics = processor.CreateGraphics();
                graphics.DrawImage (image, rectangle);
                processor.RenderNewPage
                    (
                        pageSize,
                        graphics,
                        72,
                        72
                    );
            }

            // var firstPage = new PdfFitDestination (document.Pages.First());
            // document.OpenAction = new PdfGoToAction (document, firstPage);
        }

        _logger.LogDebug ("Done {PdfName}", pdfName);

        return pdfName;
    }

    private async Task CreateHeading()
    {
        _logger.LogDebug ("Create heading...");
        await ReportMemory();
        
        using (var main = new PdfDocumentProcessor())
        {
            main.CreateEmptyDocument (_args.OutputPath);

            var document = main.Document;
            document.PageLayout = PdfPageLayout.SinglePage;
            document.PageMode = PdfPageMode.UseNone;
            document.Title = "Сшитые вместе картинки";
            document.Author = "Алексей Миронов";
            document.Subject = "Изучение DevExpress.Pdf";
            document.Keywords = "PDF JPEG DevExpress";
        }

        _logger.LogDebug ("Heading done");
    }

    private async Task MergeTail
        (
            string tailFileName
        )
    {
        var outputName = _args.OutputPath;
        var temporaryName = outputName + "_temp";
        File.Move (outputName, temporaryName);
        
        using (var processor = new PdfDocumentProcessor())
        {
            await ReportMemory();
            processor.CreateEmptyDocument(outputName);
            _logger.LogDebug ("Merging \"{Part}\"...", tailFileName);
            processor.AppendDocument (temporaryName);
            processor.AppendDocument (tailFileName);
        }
        
        File.Delete (tailFileName);
        File.Delete (temporaryName);
        _logger.LogDebug ("Done \"{Part}\"", tailFileName);

    }
    
    private async Task MergePdfs
        (
            IEnumerable<string> parts
        )
    {
        _logger.LogInformation ("Begin merge...");

        await CreateHeading();
        foreach (var part in parts)
        {
            await MergeTail (part);
        }

        _logger.LogInformation ("Merge done");
    }

    private async Task Cleanup()
    {
        _logger.LogInformation ("Begin cleanup...");
        
        Directory.Delete (_temporaryPath, true);

        await Task.CompletedTask;
        
        _logger.LogInformation ("Done cleanup");
    }

    #endregion

    #region Public methods

    public async Task<int> Run()
    {
        var inputPath = _args.InputPath;
        var outputPath = _args.OutputPath;

        _logger.LogDebug
            (
                "InputPath=\"{InputPath}\", OutputPath=\"{OutputPath}\"",
                inputPath,
                outputPath
            );

        await ReportMemory();

        File.Delete (outputPath);
        _logger.LogInformation ("Output file deleted");

        var imageFiles = await FindInputFiles (inputPath);
        if (imageFiles.Length == 0)
        {
            _logger.LogInformation ("No input files found");
            return 0;
        }

        var chunks = GroupInputFiles (imageFiles);
        var index = 0;
        var temporaryPdfs = new List<string>();
        foreach (var chunk in chunks)
        {
            var pdfName = await ProcessChunk (++index, chunk);
            temporaryPdfs.Add (pdfName);
        }

        // await MergePdfs (temporaryPdfs);
        // await Cleanup();

        await ReportMemory();

        _logger.LogInformation ("Stopping");

        return 0;
    }

    #endregion
}
