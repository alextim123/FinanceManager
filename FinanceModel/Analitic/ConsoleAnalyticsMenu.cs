using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FinanceModel.domain.entities;
using FinanceModel.Analytics;
using FinanceModel.ConsoleCreator;

namespace FinanceModel.ConsoleCreator
{
    public class ConsoleAnalyticsMenu
    {
        private readonly AnalyticsFacade _analytics;
        private readonly FinanceManager _mgr;

        public ConsoleAnalyticsMenu(AnalyticsFacade analytics, FinanceManager mgr)
        {
            _analytics = analytics;
            _mgr = mgr;
        }

        public void Run()
        {
            while (true)
            {
                Console.Clear();
                PrintMenu();
                Console.Write("> ");
                var cmd = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();

                try
                {
                    switch (cmd)
                    {
                        case "1": PeriodBalanceFlow(); Pause(); break;
                        case "2": GroupByCategoryFlow(); Pause(); break;
                        case "3": ShowLogs(); Pause(); break;
                        case "4": ClearLogs(); Pause(); break;
                        case "0": return;
                        default: PrintError("Неизвестная команда."); Pause(); break;
                    }
                }
                catch (Exception ex)
                {
                    PrintError(ex.Message);
                    Pause();
                }
            }
        }

        private void PrintMenu()
        {
            Console.WriteLine("=== Аналитика ===");
            Console.WriteLine("1) Разница доходов и расходов за период");
            Console.WriteLine("2) Группировка доходов/расходов по категориям");
            Console.WriteLine("3) Показать логи сценариев");
            Console.WriteLine("4) Очистить логи");
            Console.WriteLine("0) Назад");
            Console.WriteLine();
        }

        private void PeriodBalanceFlow()
        {
            var from = AskDate("Начало периода (день год месяц): ");
            var to = AskDate("Конец периода (день год месяц): ");

            var (income, expense, net) = _analytics.CalcPeriodBalance(from, to, timed: true, scenario: "PeriodBalance");

            PrintInfo($"Период: {from:dd yyyy MM} — {to:dd yyyy MM}");
            PrintSuccess($"Доход:  {income}");
            PrintError($"Расход: {expense}");
            WithColor(ConsoleColor.Cyan, () => Console.WriteLine($"Итог:   {net}"));
        }

        private void GroupByCategoryFlow()
        {
            var from = AskDate("Начало периода (день год месяц): ");
            var to = AskDate("Конец периода (день год месяц): ");

            var list = _analytics.GroupByCategory(from, to, timed: true, scenario: "GroupByCategory");

            if (list.Count == 0)
            {
                PrintInfo("Нет операций за период.");
                return;
            }

            Console.WriteLine("-- Доходы --");
            foreach (var x in list.Where(x => x.Type == EntryType.Income))
                PrintSuccess($"{x.CategoryName}= {x.Total}");

            Console.WriteLine("-- Расходы --");
            foreach (var x in list.Where(x => x.Type == EntryType.Expense))
                PrintError($"{x.CategoryName} {x.Total}");
        }

        private void ShowLogs()
        {
            var logs = _analytics.GetLogs();
            if (logs.Count == 0) { PrintInfo("Логи пусты."); return; }
            WithColor(ConsoleColor.Yellow, () =>
            {
                Console.WriteLine("-- Логи сценариев --");
                foreach (var l in logs) Console.WriteLine(l);
            });
        }

        private void ClearLogs()
        {
            _analytics.ClearLogs();
            PrintSuccess("Логи очищены.");
        }

        private static DateTime AskDate(string prompt)
        {
            var formats = new[] { "dd yyyy MM", "d yyyy M", "dd-yyyy-MM", "d-yyyy-M", "dd.yyyy.MM", "d.yyyy.M", "dd/yyyy/MM", "d/yyyy/M" };
            while (true)
            {
                Console.Write(prompt);
                var s = (Console.ReadLine() ?? "").Trim();
                if (DateTime.TryParseExact(s, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                    return new DateTime(dt.Year, dt.Month, dt.Day);
                PrintError("Ожидается дата в формате день год месяц, напр.: 05 2025 11.");
            }
        }

        private static void Pause()
        {
            Console.WriteLine();
            Console.Write("Нажмите любую клавишу, чтобы продолжить...");
            Console.ReadKey(true);
        }

        private static void WithColor(ConsoleColor color, Action action)
        {
            var old = Console.ForegroundColor;
            Console.ForegroundColor = color;
            try { action(); }
            finally { Console.ForegroundColor = old; }
        }

        private static void PrintSuccess(string msg) => WithColor(ConsoleColor.Green, () => Console.WriteLine(msg));
        private static void PrintError(string msg) => WithColor(ConsoleColor.Red, () => Console.WriteLine(msg));
        private static void PrintInfo(string msg) => WithColor(ConsoleColor.DarkGray, () => Console.WriteLine(msg));
    }
}
