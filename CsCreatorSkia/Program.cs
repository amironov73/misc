// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming

#region Using directives

using SkiaSharp;

#endregion

/// <summary>
/// Вся функциональность программы в одном классе.
/// </summary>
internal static class Program
{
    #region Constants

    /// <summary>
    /// Количество миниатюр по горизонтали.
    /// </summary>
    private const int HorizontalCount = 5;

    /// <summary>
    /// Количество миниатюр по вертикали.
    /// </summary>
    private const int VerticalCount = 3;

    /// <summary>
    /// Общий размер контрольного отпечатка по горизонтали.
    /// </summary>
    private const float HorizontalSize = 1920;

    /// <summary>
    /// Общий размер контрольного отпечатка по вертикали.
    /// </summary>
    private const float VerticalSize = 1080;

    /// <summary>
    /// Файлы, включаемые в контрольный отпечаток.
    /// </summary>
    private const string SourceMask = "*.jpg";

    /// <summary>
    /// Имя файла для контрольного отпечатка.
    /// </summary>
    private const string OutputPath = "!contact_sheet.jpg";

    #endregion
    
    #region Configuration

    private static string SourcePath = ".";

    #endregion

    #region Methods

    private static SKRect Inscribe
        (
            SKRect imageRect,
            SKRect outerRect
        )
    {
        var imageHeight = imageRect.Height;
        var imageWidth = imageRect.Width;
        var outerHeight = outerRect.Height;
        var outerWidth = outerRect.Width;
        var aspect = imageWidth / imageHeight / (outerWidth / outerHeight);
        var scale = aspect > 1.0f
            ? outerWidth / imageWidth
            : outerHeight / imageHeight;
        var newWidth = imageWidth * scale;
        var newHeight = imageHeight * scale;
        var offsetX = (outerWidth - newWidth) / 2.0f;
        var offsetY = (outerHeight - newHeight) / 2.0f;
        var left = outerRect.Left + offsetX;
        var top = outerRect.Top + offsetY;

        return new SKRect
            (
                left,
                top,
                left + newWidth, 
                top + newHeight
            );
    }

    private static void PutImage
        (
            SKCanvas graphics,
            string imagePath,
            int horizontalIndex,
            int verticalIndex
        )
    {
        const float cellWidth = HorizontalSize / HorizontalCount;
        const float cellHeight = VerticalSize / VerticalCount;
        var left = cellWidth * horizontalIndex;
        var top = cellHeight * verticalIndex;
        var cell = new SKRect
            (
                left,
                top,
                left + cellWidth,
                top + cellHeight
            );

        using var bitmap = SKBitmap.Decode (imagePath);
        var source = new SKRect (0, 0, bitmap.Width, bitmap.Height);
        var target = Inscribe (source, cell);
        graphics.DrawBitmap (bitmap, target);
    }

    private static void CreateContactSheet()
    {
        var files = Directory.GetFiles 
            (
                SourcePath,
                SourceMask,
                SearchOption.TopDirectoryOnly
            );
        if (files.Length <= HorizontalCount)
        {
            return;
        }

        var array = files
            .Where (x => Path.GetFileName (x).ToLower() != OutputPath)
            .ToArray();

        var selectedFiles = array
            .Where (x => Path.GetFileNameWithoutExtension (x).Contains ('!'))
            .ToList();

        if (Path.GetFileNameWithoutExtension (array[0]).Contains ('!') && !selectedFiles.Contains (array[0]))
        {
            selectedFiles.Insert (0, array[0]);
        }

        const int totalCount = VerticalCount * HorizontalCount;
        if (selectedFiles.Count >= array.Length / 2 || selectedFiles.Count >= totalCount / 2)
        {
            var list = array
                .Where (x => !selectedFiles.Contains (x))
                .Take (totalCount - selectedFiles.Count).ToList();
            selectedFiles.AddRange (list);
            array = selectedFiles.ToArray();
        }

        var num2 = Math.Max (array.Length / (HorizontalCount * VerticalCount), 1);
        
        // создаем поверхность для рисования
        var imageInfo = new SKImageInfo 
            (
                (int) HorizontalSize,
                (int) VerticalSize
            );
        using var surface = SKSurface.Create(imageInfo);
         
        // очищаем ее
        // для этого сначала нужно получить канву 
        using var canvas = surface.Canvas;
        
        // затем залить ее белым цветом
        canvas.Clear (SKColors.White);
        
        var index = 0;
        for (var verticalIndex = 0; verticalIndex < VerticalCount; ++verticalIndex)
        {
            for (
                    var horizontalIndex = 0;
                    horizontalIndex < HorizontalCount && index < array.Length;
                    index += num2
                )
            {
                Console.Write ("{0} ", index);
                PutImage (canvas, array[index], horizontalIndex, verticalIndex);
                ++horizontalIndex;
            }
        }

        // теперь всё это богатство необходимо сохранить
        var filename = Path.Combine (SourcePath, OutputPath);
        File.Delete (filename);
        
        // создаем растровый экземпляр поверхности
        var image = surface.Snapshot();
         
        // кодируем его в формат PNG
        var data = image.Encode (SKEncodedImageFormat.Jpeg, 90);
         
        // вот так нетривиально изображение записывается в файл
        using var stream = File.Create (filename);
        data.SaveTo(stream);
    }

    private static void Write
        (
            ConsoleColor color,
            string text
        )
    {
        var saveForeground = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.Write (text);
        Console.ForegroundColor = saveForeground;
    }

    private static void WriteLine
        (
            ConsoleColor color,
            string text
        )
    {
        var saveForeground = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine (text);
        Console.ForegroundColor = saveForeground;
    }

    #endregion
    
    /// <summary>
    /// Точка входа.
    /// </summary>
    public static int Main (string[] args)
    {
        if (args.Length != 0)
        {
            SourcePath = args[0];
        }

        Write (ConsoleColor.Yellow, $"{SourcePath} ");
        CreateContactSheet();
        WriteLine (ConsoleColor.Green, "done");

        var directories = Directory.GetDirectories
            (
                SourcePath, 
                "*.*", 
                SearchOption.AllDirectories
            );
        foreach (var directory in directories)
        {
            SourcePath = directory;
            Write (ConsoleColor.Yellow, $"{SourcePath} ");
            CreateContactSheet();
            WriteLine (ConsoleColor.Green, "done");
        }

        return 0;
    }
}
