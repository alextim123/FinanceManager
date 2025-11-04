using System;
using FinanceModel.ConsoleCreator;
using FinanceModel.ConsoleMenus;
using FinanceModel.Analytics;

namespace FinanceModel.App
{
    public sealed class ConsoleMainMenu
    {
        private readonly ConsoleImportSubmenu _importMenu;
        private readonly ExportSubMenu _exportMenu;
        private readonly ConsoleAnalyticsMenu _analyticsMenu;
        private readonly ConsoleCreator.ConsoleCreator _crudMenu;

        public ConsoleMainMenu(ConsoleImportSubmenu importMenu, ExportSubMenu exportMenu, ConsoleAnalyticsMenu analyticsMenu, ConsoleCreator.ConsoleCreator crudMenu)
        {
            _importMenu = importMenu;
            _exportMenu = exportMenu;
            _analyticsMenu = analyticsMenu;
            _crudMenu = crudMenu;
        }

        public void Run()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Главное меню ===");
                Console.WriteLine("1) Импорт данных");
                Console.WriteLine("2) Экспорт данных");
                Console.WriteLine("3) Аналитика");
                Console.WriteLine("4) Ручное добавление/редактирование");
                Console.WriteLine("0) Выход");
                Console.Write("> ");
                var cmd = (Console.ReadLine() ?? "").Trim();

                switch (cmd)
                {
                    case "1": _importMenu.Run(); break;
                    case "2": _exportMenu.Run(); break;
                    case "3": _analyticsMenu.Run(); break;
                    case "4": _crudMenu.Run(); break;
                    case "0": return;
                    default:
                        PrintErr("Неизвестная команда."); Pause();
                        break;
                }
            }
        }

        private static void WithColor(ConsoleColor c, Action a)
        {
            var o = Console.ForegroundColor;
            Console.ForegroundColor = c;
            try { a(); } finally { Console.ForegroundColor = o; }
        }
        private static void PrintErr(string s) => WithColor(ConsoleColor.Red, () => Console.WriteLine(s));
        private static void Pause() { Console.WriteLine(); Console.Write("Нажмите любую клавишу..."); Console.ReadKey(true); }
    }
}
