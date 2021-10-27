using System;
using System.IO;

using WebPWrapper;
using WebPWrapper.Decoder;

class Program
{
    static int Main (string[] args)
    {
        if (args.Length is < 1 or > 2)
        {
            Console.Error.WriteLine ("Usage: webp-to-jpeg <input> [output]");
            return 1;
        }

        try
        {

            var inputFileName = args[0];
            var outputFileName = args.Length > 1
                ? args[1]
                : Path.ChangeExtension (inputFileName, ".jpg");

            WebPExecuteDownloader.Download();

            var builder = new WebPDecoderBuilder();
            var decoder = builder.Build();

            using var output = File.Create (outputFileName);
            using var input = File.OpenRead (inputFileName);
            decoder.Decode (input, output);

        }
        catch (Exception exception)
        {
            Console.Error.WriteLine ($"Exception: {exception.Message}");
            return 1;
        }

        return 0;
    }
}
