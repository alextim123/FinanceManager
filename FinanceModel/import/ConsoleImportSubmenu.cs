using FinanceModel.import;
using FinanceModel.Import;
using System;
using System.IO;

namespace FinanceModel.ConsoleMenus
{
    public sealed class ConsoleImportSubmenu
    {
        private readonly ImportFacade _facade;

        public ConsoleImportSubmenu(ImportFacade facade)
        {
            _facade = facade;
        }

        public void Run()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Импорт из файла ===");
                Console.WriteLine("1) CSV");
                Console.WriteLine("2) JSON");
                Console.WriteLine("0) Назад");
                Console.Write("> ");
                var cmd = (Console.ReadLine() ?? "").Trim();

                if (cmd == "0") return;

                ImportStrategy? strategy = cmd switch
                {
                    "1" => new CsvImportStrategy(),
                    "2" => new JsonImportStrategy(),
                    _ => null
                };

                if (strategy is null)
                {
                    PrintError("Неизвестный формат.");
                    Pause();
                    continue;
                }

                var allowedExt = (cmd == "1") ? [".csv"] : new[] { ".json" };
                var path = AskFilePath("Путь к файлу: ", allowedExt);

                var report = _facade.ImportFromFile(strategy, path);

                Console.Clear();
                PrintGood("Импорт завершён");
                foreach (var line in ImportFacade.BuildSummaryLines(report))
                    PrintInfo(line);

                if (report.Errors.Count > 0)
                {
                    PrintWarn("-- Ошибки --");
                    foreach (var e in ImportFacade.BuildErrorLines(report))
                        PrintError(e);
                }

                Pause();
            }
        }

        private static string AskFilePath(string prompt, string[] allowedExt)
        {
            while (true)
            {
                Console.Write(prompt);
                var raw = Console.ReadLine() ?? "";
                raw = raw.Trim().Trim('"');

                if (string.IsNullOrWhiteSpace(raw))
                {
                    PrintError("Поле не должно быть пустым.");
                    continue;
                }

                string full;
                try { full = Path.GetFullPath(raw); }
                catch
                {
                    PrintError("Некорректный путь.");
                    continue;
                }

                if (!File.Exists(full))
                {
                    PrintError("Файл не найден.");
                    continue;
                }

                var ext = Path.GetExtension(full);
                var okExt = false;
                foreach (var e in allowedExt)
                    if (string.Equals(ext, e, StringComparison.OrdinalIgnoreCase)) { okExt = true; break; }

                if (!okExt)
                {
                    PrintError($"Ожидается файл с расширением: {string.Join(", ", allowedExt)}");
                    continue;
                }

                return full;
            }
        }

        private static void WithColor(ConsoleColor color, Action action)
        {
            var old = Console.ForegroundColor;
            Console.ForegroundColor = color;
            try { action(); } finally { Console.ForegroundColor = old; }
        }

        private static void PrintGood(string s) => WithColor(ConsoleColor.Green, () => Console.WriteLine(s));
        private static void PrintInfo(string s) => WithColor(ConsoleColor.Gray, () => Console.WriteLine(s));
        private static void PrintWarn(string s) => WithColor(ConsoleColor.Yellow, () => Console.WriteLine(s));
        private static void PrintError(string s) => WithColor(ConsoleColor.Red, () => Console.WriteLine(s));

        private static void Pause()
        {
            Console.WriteLine();
            Console.Write("Нажмите любую клавишу...");
            Console.ReadKey(true);
        }
    }
}
