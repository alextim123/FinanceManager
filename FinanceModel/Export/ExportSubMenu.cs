using FinanceModel.Export;
using FinanceModel.Export.Intefaces;
using System;

namespace FinanceModel.ConsoleMenus
{
    public sealed class ExportSubMenu
    {
        private readonly ExportFacade _facade;

        public ExportSubMenu(ExportFacade facade) { _facade = facade; }

        public void Run()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Экспорт данных ===");
                Console.WriteLine("1) В CSV");
                Console.WriteLine("2) В JSON");
                Console.WriteLine("0) Назад");
                Console.Write("> ");
                var cmd = (Console.ReadLine() ?? "").Trim();

                if (cmd == "0") return;

                IExportVisitor? visitor = null;
                string ext = "";
                switch (cmd)
                {
                    case "1": visitor = new CsvExportVisitor(); ext = ".csv"; break;
                    case "2": visitor = new JsonExportVisitor(); ext = ".json"; break;
                }
                if (visitor is null)
                {
                    PrintError("Неизвестный формат.");
                    Pause();
                    continue;
                }

            
                string name;
                while (true)
                {
                    Console.Write($"Имя файла без расширения (будет сохранён в {ExportStorage.Dir}): ");
                    name = (Console.ReadLine() ?? "").Trim();
                    if (!string.IsNullOrWhiteSpace(name)) break;
                    PrintError("Имя файла не должно быть пустым.");
                }

                var path = ExportStorage.MakePath(name, ext);

                try
                {
                    _facade.ExportAll(visitor, path);
                    PrintOk($"Экспорт завершён: {path}");
                }
                catch (Exception ex)
                {
                    PrintError(ex.Message);
                }
                Pause();
            }
        }

        private static void PrintOk(string s) => WithColor(ConsoleColor.Green, () => Console.WriteLine(s));
        private static void PrintError(string s) => WithColor(ConsoleColor.Red, () => Console.WriteLine(s));
        private static void Pause() { Console.WriteLine(); Console.Write("Нажмите любую клавишу..."); Console.ReadKey(true); }
        private static void WithColor(ConsoleColor color, Action action)
        {
            var old = Console.ForegroundColor;
            Console.ForegroundColor = color;
            try { action(); } finally { Console.ForegroundColor = old; }
        }
    }
}
