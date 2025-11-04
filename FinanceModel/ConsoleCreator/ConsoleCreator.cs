using FinanceModel.domain.entities;
using FinanceModel.Domain.Fabric.Interfaces;
using System;
using System.Globalization;
using System.Linq;

namespace FinanceModel.ConsoleCreator
{
    public class ConsoleCreator
    {
        private readonly FinanceManager _mgr;
        private readonly IBankAccountFactory _accFactory;
        private readonly ICategoryFactory _catFactory;
        private readonly IOperationFactory _opFactory;

        public ConsoleCreator(FinanceManager mgr, IBankAccountFactory accFactory, ICategoryFactory catFactory, IOperationFactory opFactory)
        {
            _mgr = mgr;
            _accFactory = accFactory;
            _catFactory = catFactory;
            _opFactory = opFactory;
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
                        case "1": ListAccounts(); Pause(); break;
                        case "2": ListCategories(); Pause(); break;
                        case "3": ListOperations(); Pause(); break;

                        case "4": AddAccountFlow(); Pause(); break;
                        case "5": AddCategoryFlow(); Pause(); break;
                        case "6": AddOperationFlow(); Pause(); break;

                        case "7": ReplaceAccountFlow(); Pause(); break;
                        case "8": ReplaceCategoryFlow(); Pause(); break;
                        case "9": ReplaceOperationFlow(); Pause(); break;

                        case "10": DeleteAccountFlow(); Pause(); break;
                        case "11": DeleteCategoryFlow(); Pause(); break;
                        case "12": DeleteOperationFlow(); Pause(); break;

                        case "0": return;
                        default:
                            PrintError("Неизвестная команда.");
                            Pause();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    PrintError($"Ошибка: {ex.Message}");
                    Pause();
                }
            }
        }

        private void PrintMenu()
        {
            Console.WriteLine("=== Финансы: подменю ===");
            Console.WriteLine("1) Показать все Счета");
            Console.WriteLine("2) Показать все Категории");
            Console.WriteLine("3) Показать все Операции");
            Console.WriteLine("4) Добавить Счёт");
            Console.WriteLine("5) Добавить Категорию");
            Console.WriteLine("6) Добавить Операцию");
            Console.WriteLine("7) Заменить Счёт по ID");
            Console.WriteLine("8) Заменить Категорию по ID");
            Console.WriteLine("9) Заменить Операцию по ID");
            Console.WriteLine("10) Удалить Счёт по ID");
            Console.WriteLine("11) Удалить Категорию по ID");
            Console.WriteLine("12) Удалить Операцию по ID");
            Console.WriteLine("0) Назад");
            Console.WriteLine();
        }

        // Вывод списка сущностей
        private void ListAccounts()
        {
            var list = _mgr.GetAllBankAccounts();
            if (list.Count == 0) { PrintInfo("Счета пусты."); return; }
            Console.WriteLine("-- Счета --");
            foreach (var a in list) Console.WriteLine($"{a.ID} | {a.Name} | Баланс: {a.Balance}");
        }

        private void ListCategories()
        {
            var list = _mgr.GetAllCategories();
            if (list.Count == 0) { PrintInfo("Категории пусты."); return; }
            Console.WriteLine("-- Категории --");
            foreach (var c in list) Console.WriteLine($"{c.ID} | {c.Name} | {c.Type}");
        }

        private void ListOperations()
        {
            var list = _mgr.GetAllOperations();
            if (list.Count == 0) { PrintInfo("Операций нет."); return; }
            Console.WriteLine("-- Операции --");
            foreach (var o in list)
                Console.WriteLine($"{o.ID} | {o.Type} | {o.Account.ID}->{o.Category.ID} | {o.Amount} | {o.Date:dd yyyy MM} | {o.Description}");
        }

        // Добавление сущности
        private void AddAccountFlow()
        {
            var id = Ask("ID счёта: ");
            var name = Ask("Название счёта: ");
            var balance = AskInt("Начальный баланс (int): ");

            try
            {
                var acc = _accFactory.Create(id, name, balance);
                var (ok, err) = _mgr.TryAddBankAccount(acc);
                if (ok) PrintSuccess("Счёт добавлен.");
                else PrintError(err ?? "Не удалось добавить.");
            }
            catch (Exception ex) { PrintError(ex.Message); }
        }

        private void AddCategoryFlow()
        {
            var id = Ask("ID категории: ");
            var name = Ask("Название категории: ");
            var type = AskType("Тип (доход/расход): ");

            try
            {
                var cat = _catFactory.Create(id, name, type);
                var (ok, err) = _mgr.TryAddCategory(cat);
                if (ok) PrintSuccess("Категория добавлена.");
                else PrintError(err ?? "Не удалось добавить.");
            }
            catch (Exception ex) { PrintError(ex.Message); }
        }

        private void AddOperationFlow()
        {
            var id = Ask("ID операции: ");
            var type = AskType("Тип операции (доход/расход): ");
            var accId = Ask("ID счёта: ");
            var catId = Ask("ID категории: ");
            var amount = AskInt("Сумма (int > 0): ");
            var dateDym = Ask("Дата (день год месяц, напр. 05 2025 11): ");
            var desc = Ask("Описание (опционально): ", allowEmpty: true);

            var acc = _mgr.GetAllBankAccounts().FirstOrDefault(a => a.ID == accId);
            if (acc is null) { PrintError($"Счёт '{accId}' не найден."); return; }
            var cat = _mgr.GetAllCategories().FirstOrDefault(c => c.ID == catId);
            if (cat is null) { PrintError($"Категория '{catId}' не найдена."); return; }

            try
            {
                var op = _opFactory.Create(id, type, acc, cat, amount, dateDym, string.IsNullOrWhiteSpace(desc) ? null : desc);
                var (ok, err) = _mgr.TryAddOperation(op);
                if (ok) PrintSuccess("Операция добавлена.");
                else PrintError(err ?? "Не удалось добавить.");
            }
            catch (Exception ex) { PrintError(ex.Message); }
        }

        // Замена сущности
        private void ReplaceAccountFlow()
        {
            var id = Ask("ID счёта (замена по этому ID): ");
            var name = Ask("Новое название: ");
            var balance = AskInt("Новый баланс (int): ");

            try
            {
                var acc = _accFactory.Create(id, name, balance);
                var (ok, err) = _mgr.TryReplaceBankAccount(acc);
                if (ok) PrintSuccess("Счёт заменён.");
                else PrintError(err ?? "Не удалось заменить.");
            }
            catch (Exception ex) { PrintError(ex.Message); }
        }

        private void ReplaceCategoryFlow()
        {
            var id = Ask("ID категории (замена по этому ID): ");
            var name = Ask("Новое название: ");
            var type = AskType("Новый тип (доход/расход): ");

            try
            {
                var cat = _catFactory.Create(id, name, type);
                var (ok, err) = _mgr.TryReplaceCategory(cat);
                if (ok) PrintSuccess("Категория заменена.");
                else PrintError(err ?? "Не удалось заменить.");
            }
            catch (Exception ex) { PrintError(ex.Message); }
        }

        private void ReplaceOperationFlow()
        {
            var id = Ask("ID операции (замена по этому ID): ");
            var type = AskType("Новый тип операции (доход/расход): ");
            var accId = Ask("Новый ID счёта: ");
            var catId = Ask("Новый ID категории: ");
            var amount = AskInt("Новая сумма (int > 0): ");
            var dateDym = Ask("Новая дата (день год месяц): ");
            var desc = Ask("Новое описание (опционально): ", allowEmpty: true);

            var acc = _mgr.GetAllBankAccounts().FirstOrDefault(a => a.ID == accId);
            if (acc is null) { PrintError($"Счёт '{accId}' не найден."); return; }
            var cat = _mgr.GetAllCategories().FirstOrDefault(c => c.ID == catId);
            if (cat is null) { PrintError($"Категория '{catId}' не найдена."); return; }

            try
            {
                var op = _opFactory.Create(id, type, acc, cat, amount, dateDym, string.IsNullOrWhiteSpace(desc) ? null : desc);
                var (ok, err) = _mgr.TryReplaceOperation(op);
                if (ok) PrintSuccess("Операция заменена.");
                else PrintError(err ?? "Не удалось заменить.");
            }
            catch (Exception ex) { PrintError(ex.Message); }
        }

        // Удаление сущностей
        private void DeleteAccountFlow()
        {
            var id = Ask("ID счёта для удаления: ");
            var (ok, err) = _mgr.TryRemoveBankAccountById(id);
            if (ok) PrintSuccess("Счёт удалён.");
            else PrintError(err ?? "Не удалось удалить.");
        }

        private void DeleteCategoryFlow()
        {
            var id = Ask("ID категории для удаления: ");
            var (ok, err) = _mgr.TryRemoveCategoryById(id);
            if (ok) PrintSuccess("Категория удалена.");
            else PrintError(err ?? "Не удалось удалить.");
        }

        private void DeleteOperationFlow()
        {
            var id = Ask("ID операции для удаления: ");
            var (ok, err) = _mgr.TryRemoveOperationById(id);
            if (ok) PrintSuccess("Операция удалена.");
            else PrintError(err ?? "Не удалось удалить.");
        }

        private static string Ask(string prompt, bool allowEmpty = false)
        {
            while (true)
            {
                Console.Write(prompt);
                var s = Console.ReadLine() ?? "";
                if (allowEmpty || !string.IsNullOrWhiteSpace(s)) return s.Trim();
                PrintError("Поле не должно быть пустым.");
            }
        }

        private static int AskInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var s = Console.ReadLine();
                if (int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v))
                    return v;
                PrintError("Введите целое число (int).");
            }
        }

        private static EntryType AskType(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var s = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();
                if (s.StartsWith("i") || s == "доход") return EntryType.Income;
                if (s.StartsWith("e") || s == "расход") return EntryType.Expense;
                PrintError("Введите 'доход' или 'расход'.");
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
