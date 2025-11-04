using FinanceModel.ConsoleCreator;
using FinanceModel.domain.entities;
using FinanceModel.Domain.Fabric.Interfaces;
using FinanceModel.import;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace FinanceModel.Import
{
    public sealed class ImportReport
    {
        public int AccountsOk, AccountsSkip;
        public int CategoriesOk, CategoriesSkip;
        public int OperationsOk, OperationsSkip;
        public readonly List<string> Errors = new();
    }

    public interface IErrorData
    {
        void Add(string line);
        List<string> GetAll();
        void Clear();
    }

    public sealed class ImportFacade
    {
        private readonly FinanceManager _mgr;
        private readonly IBankAccountFactory _accF;
        private readonly ICategoryFactory _catF;
        private readonly IOperationFactory _opF;
        private readonly IErrorData? _log;

        public ImportReport? LastReport { get; private set; }

        public ImportFacade(
            FinanceManager mgr,
            IBankAccountFactory accFactory,
            ICategoryFactory catFactory,
            IOperationFactory opFactory,
            IErrorData? errorLog = null)
        {
            _mgr = mgr;
            _accF = accFactory;
            _catF = catFactory;
            _opF = opFactory;
            _log = errorLog;
        }
        public ImportReport ImportFromFile(ImportStrategy strategy, string path)
        {
            var report = new ImportReport();

            _mgr.RemoveAllBankAccounts();
            _mgr.RemoveAllCategories();
            _mgr.RemoveAllOperations();

            List<Operation> ops;
            List<Category> cats;
            List<BankAccount> accs;

            try
            {
                (ops, cats, accs) = strategy.TemplateParse(path);
            }
            catch (Exception ex)
            {
                report.Errors.Add($"parse:{ex.Message}");
                _log?.Add($"[IMPORT] parse error: {ex.Message}");
                LastReport = report;
                return report;
            }

            //Accounts
            foreach (var a in accs)
            {
                try
                {
                    var acc = _accF.Create(a.ID, a.Name, a.Balance);
                    var (ok, err) = _mgr.TryAddBankAccount(acc);
                    if (ok)
                    {
                        report.AccountsOk++;
                    }
                    else
                    {
                        report.AccountsSkip++;
                        if (!string.IsNullOrWhiteSpace(err))
                        {
                            var msg = $"acc:{a.ID} {err}";
                            report.Errors.Add(msg);
                            _log?.Add($"[IMPORT] {msg}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    report.AccountsSkip++;
                    var msg = $"acc:{a.ID} {ex.Message}";
                    report.Errors.Add(msg);
                    _log?.Add($"[IMPORT] {msg}");
                }
            }

            //Categories
            foreach (var c in cats)
            {
                try
                {
                    var cat = _catF.Create(c.ID, c.Name, c.Type);
                    var (ok, err) = _mgr.TryAddCategory(cat);
                    if (ok)
                    {
                        report.CategoriesOk++;
                    }
                    else
                    {
                        report.CategoriesSkip++;
                        if (!string.IsNullOrWhiteSpace(err))
                        {
                            var msg = $"cat:{c.ID} {err}";
                            report.Errors.Add(msg);
                            _log?.Add($"[IMPORT] {msg}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    report.CategoriesSkip++;
                    var msg = $"cat:{c.ID} {ex.Message}";
                    report.Errors.Add(msg);
                    _log?.Add($"[IMPORT] {msg}");
                }
            }

            //Operations
            foreach (var o in ops)
            {
                try
                {
                    var acc = _mgr.GetAllBankAccounts().FirstOrDefault(x => x.ID == o.Account.ID);
                    if (acc is null)
                    {
                        report.OperationsSkip++;
                        var msg = $"operation:{o.ID} account '{o.Account.ID}' не найден";
                        report.Errors.Add(msg);
                        _log?.Add($"[IMPORT] {msg}");
                        continue;
                    }

                    var cat = _mgr.GetAllCategories().FirstOrDefault(x => x.ID == o.Category.ID);
                    if (cat is null)
                    {
                        report.OperationsSkip++;
                        var msg = $"operation:{o.ID} category '{o.Category.ID}' не найден";
                        report.Errors.Add(msg);
                        _log?.Add($"[IMPORT] {msg}");
                        continue;
                    }

                    var dateDym = o.Date.ToString("dd yyyy MM", CultureInfo.InvariantCulture);
                    var op = _opF.Create(o.ID, o.Type, acc, cat, o.Amount, dateDym, o.Description);

                    var (ok, err) = _mgr.TryAddOperation(op);
                    if (ok)
                    {
                        report.OperationsOk++;
                    }
                    else
                    {
                        report.OperationsSkip++;
                        if (!string.IsNullOrWhiteSpace(err))
                        {
                            var msg = $"op:{o.ID} {err}";
                            report.Errors.Add(msg);
                            _log?.Add($"[IMPORT] {msg}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    report.OperationsSkip++;
                    var msg = $"op:{o.ID} {ex.Message}";
                    report.Errors.Add(msg);
                    _log?.Add($"[IMPORT] {msg}");
                }
            }

            LastReport = report;
            return report;
        }

        
        public static List<string> BuildSummaryLines(ImportReport r)
        {
            return new List<string>
            {
                $"Accounts:   OK={r.AccountsOk}  Пропущено={r.AccountsSkip}",
                $"Categories: OK={r.CategoriesOk}  Пропущено={r.CategoriesSkip}",
                $"Operations: OK={r.OperationsOk}  Пропущено={r.OperationsSkip}"
            };
        }

       
        public static List<string> BuildErrorLines(ImportReport r)
        {
            return r.Errors.ToList(); 
        }
    }
}
