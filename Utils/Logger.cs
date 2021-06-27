using System;

namespace FadeBot
{
    public static class Logger
    {
        private static int tableWidth = 73;

        public static void Log(object obj)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("[FadeBot] " + obj);
        }

        public static void LogDebug(object obj)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("[FadeBot] " + obj);
        }

        public static void LogImportant(object obj)
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("[FadeBot] " + obj);
        }

        public static void LogSuccess(object obj)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("[FadeBot] " + obj);
        }

        public static void LogError(object obj)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[FadeBot] " + obj);
        }

        public static void LogWarning(object obj)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("[FadeBot] " + obj);
        }

        public static void PrintRow(params string[] columns)
        {
            int width = (tableWidth - columns.Length) / columns.Length;
            string row = "|";

            foreach (string column in columns)
            {
                row += AlignCentre(column, width) + "|";
            }
            Log(row);
        }

        private static string AlignCentre(string text, int width)
        {
            text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;
            if (string.IsNullOrEmpty(text)) return new string(' ', width);
            else return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
        }
    }
}