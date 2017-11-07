using System;
using System.IO;
using System.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ImageToConsole
{
    internal class Program
    {
        private static readonly string[] Characters = {" ", ".", "-", ":", "*", "+", "=", "%", "@", "#", "#"};

        private static void Main(string[] args)
        {
            var width = Math.Min(Console.BufferWidth, Console.WindowWidth);
            var height = Math.Min(Console.BufferHeight, Console.WindowHeight) - 1;
            var useColors = args.Any(a => a.ToLower() == "-c");
            var filename = args.SingleOrDefault(a => a.ToLower() != "-c") ?? "default.png";

            using (var inputStream = File.OpenRead(filename))
            using (var image = Image.Load<Rgba32>(inputStream))
            {
                image.Mutate(x => x.Resize(Math.Min(width, image.Width), Math.Min(height, image.Height)));

                Console.BackgroundColor = ConsoleColor.Black;
                for (var x = 0; x < image.Width; x++)
                for (var y = 0; y < image.Height; y++)
                {
                    Console.SetCursorPosition(x, y);

                    if (useColors)
                        Console.ForegroundColor = GetColor(image[x, y].Rgb);

                    Console.Write(Characters[(image[x, y].B * Characters.Length - 1) / 255]);
                }
            }

            Console.ReadKey();
            Console.Clear();
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
    }
}