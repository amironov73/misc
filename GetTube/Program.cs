// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// ReSharper disable CheckNamespace

/* Program.cs -- program entry point
 * Ars Magna project, http://arsmagna.ru
 */

#region Using directives

using System;

using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Threading.Tasks;

using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

#endregion

#nullable enable

namespace GetTube;

/*
 * Простая программа для скачивания видео с YouTube.
 */

internal static class Program
{
    private static Argument<string> _urlArgument = new ("url", "Youtube video URL")
    {
        Arity = ArgumentArity.ExactlyOne
    };
    private static Option<string> _outputOption = new ("--output", "output file");
    
    public static int Main 
        (
            string[] args
        )
    {
        var rootCommand = new RootCommand ("Youtube video download");
        rootCommand.AddArgument (_urlArgument);
        rootCommand.AddOption (_outputOption);
        rootCommand.Description = "Youtube video downloader";
        rootCommand.SetHandler ((Action<ParseResult>) Run);

        try
        {
            new CommandLineBuilder (rootCommand)
                .UseDefaults()
                .Build()
                .Invoke (args);
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine (exception);
            return 1;
        }
        
        return 0;
    }

    private static async Task Download 
        (
            DownloadParameters parameters
        )
    {
        var videoUrl = parameters.Url;
        if (string.IsNullOrEmpty (videoUrl))
        {
            throw new ArgumentNullException (nameof (parameters));
        }
        
        var youtube = new YoutubeClient();
        var videoId = VideoId.Parse (videoUrl);

        var video = await youtube.Videos.GetAsync (videoUrl);
        var title = video.Title;
        Console.WriteLine (title);

        var streamManifest = await youtube.Videos.Streams.GetManifestAsync (videoId);
        var streamInfo = streamManifest.GetMuxedStreams().TryGetWithHighestVideoQuality();
        if (streamInfo is null)
        {
            await Console.Error.WriteLineAsync ("Error!");
            return;
        }
        
        var fileName = parameters.Output ?? $"{videoId}.{streamInfo.Container.Name}";
        using (var progress = new YoutubeProgress())
        {
            await youtube.Videos.Streams.DownloadAsync 
                (
                    streamInfo,
                    fileName, 
                    progress
                );
        }
    }

    private static void Run
        (
            ParseResult parseResult
        )
    {
        var parameters = new DownloadParameters()
        {
            Url = parseResult.GetValueForArgument (_urlArgument),
            Output = parseResult.GetValueForOption (_outputOption)
        };

        Download (parameters).GetAwaiter().GetResult();
    }
}
