using System;
using System.IO;
using System.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ImageToConsole
{
    internal class Program
    {
        private static readonly string[] Characters = { "#", "@", "%", "=", "+", "*", ":", "-", ".", " " };

        private static void Main(string[] args)
        {
            var parameters = ParseParameters(args);

            var initialBackgroundColor = Console.BackgroundColor;
            var initialForegroundColor = Console.ForegroundColor;

            var consolewidth = Math.Min(Console.BufferWidth, Console.WindowWidth);
            var consoleHeight = Math.Min(Console.BufferHeight, Console.WindowHeight) - 1;

            using (var inputStream = File.OpenRead(parameters.FileName))
            using (var image = Image.Load<Rgba32>(inputStream))
            {
                if (parameters.ScaleImage)
                    ScaleImage(image, consolewidth, consoleHeight);
                else
                    image.Mutate(x =>
                        x.Resize(Math.Min(consolewidth, image.Width), Math.Min(consoleHeight, image.Height)));

                Console.BackgroundColor = ConsoleColor.Black;
                Console.Clear();
                for (var x = 0; x < image.Width; x++)
                for (var y = 0; y < image.Height; y++)
                {
                    Console.SetCursorPosition(x, y);
                    var pixel = image[x, y];

                    if (parameters.UseColors)
                        Console.ForegroundColor = GetColor(pixel.Rgb);

                    var pixelBrightness = ((pixel.R + pixel.G + pixel.B) / 3 + pixel.A) / 2;

                    if (pixel.A < 50)
                        pixelBrightness = 255;

                    var characterForPixel = Characters[(pixelBrightness * Characters.Length - 1) / 255];
                    Console.Write(characterForPixel);
                }
            }

            Console.ReadKey();
            Console.BackgroundColor = initialBackgroundColor;
            Console.ForegroundColor = initialForegroundColor;
            Console.Clear();
        }

        private static Parameters ParseParameters(string[] args)
        {
            var parameters = new Parameters();

            var commands = args.ToList();

            parameters.UseColors = commands.Any(a => a.ToLower() == "-c");
            commands.Remove("-c");

            parameters.ScaleImage = commands.Any(a => a.ToLower() == "-s");
            commands.Remove("-s");

            parameters.FileName = commands.SingleOrDefault() ?? "default.png";

            return parameters;
        }

        public static void ScaleImage(Image<Rgba32> image, int maxWidth, int maxHeight)
        {
            var ratioX = (double) maxWidth / image.Width;
            var ratioY = (double) maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var targetWidth = (int) (image.Width * ratio);
            var targetHeight = (int) (image.Height * ratio);

            image.Mutate(x => x.Resize(targetWidth, targetHeight));
        }

        private static ConsoleColor GetColor(Rgb24 rgb)
        {
            if (rgb.R < 100 && rgb.G < 100 && rgb.B < 100)
                return ConsoleColor.DarkGray;

            if (rgb.R > 155 && rgb.G > 155 && rgb.B > 155)
                return ConsoleColor.White;

            if (rgb.R > rgb.G && rgb.R > rgb.B)
                return ConsoleColor.Red;

            if (rgb.G > rgb.R && rgb.G > rgb.B)
                return ConsoleColor.Green;

            if (rgb.B > rgb.R && rgb.B > rgb.G)
                return ConsoleColor.Blue;

            return ConsoleColor.Gray;
        }

        private struct Parameters
        {
            public bool UseColors { get; set; }
            public bool ScaleImage { get; set; }
            public string FileName { get; set; }
        }
    }
}