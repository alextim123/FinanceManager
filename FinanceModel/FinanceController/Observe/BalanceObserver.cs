using FinanceModel.ConsoleCreator;
using FinanceModel.domain.entities;
using System;
using System.Linq;

public sealed class BalanceObserver
{
    private readonly FinanceManager _mgr;

    public BalanceObserver(FinanceManager mgr) => _mgr = mgr;

    public void OnAdded(Operation op) => Recalc(op.Account.ID);
    public void OnRemoved(Operation op) => Recalc(op.Account.ID);
    public void OnReplaced(Operation _, Operation newOp) => Recalc(newOp.Account.ID);

    private void Recalc(string accountId)
    {
        var acc = _mgr.GetAllBankAccounts().FirstOrDefault(a => a.ID == accountId);
        if (acc is null)
        {
            Console.WriteLine($"[RECALC] account '{accountId}' не найден (баланс не проверён)");
            return;
        }

        var opsAll = _mgr.GetAllOperations();
        int income = opsAll.Where(o => o.Account.ID == accountId && o.Type == EntryType.Income).Sum(o => o.Amount);
        int expense = opsAll.Where(o => o.Account.ID == accountId && o.Type == EntryType.Expense).Sum(o => o.Amount);
        int computed = income - expense;
        int delta = computed - acc.Balance;

        Console.WriteLine($"[RECALC] acc={accountId} был пересчитан баланс");
    }
}
