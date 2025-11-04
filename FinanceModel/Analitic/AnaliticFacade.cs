using FinanceModel.Analitic;
using FinanceModel.domain.entities;
using FinanceModel.ErrorData;
using FinanceModel.ConsoleCreator;   
using System;
using System.Collections.Generic;

namespace FinanceModel.Analytics
{
    public sealed class AnalyticsFacade
    {
        private readonly IErrorData _store;
        private readonly Invoker _invoker = new Invoker();
        private readonly FinanceManager _mgr;

        public AnalyticsFacade(FinanceManager manager, IErrorData store)
        {
            _mgr = manager;
            _store = store;
        }

        public (int income, int expense, int net) CalcPeriodBalance(
            DateTime from, DateTime to, bool timed, string scenario)
        {
            var ops = _mgr.GetAllOperations();
            var receiver = new AnalyticsReceiver(ops);
            Command baseCmd = new CalcPeriodBalanceCommand(receiver, from, to);

            if (timed && _store != null)
                _invoker.SetTimedLogged(baseCmd, _store, scenario);
            else
                _invoker.SetCommand(baseCmd);

            _invoker.Run();
            return (receiver.LastIncome, receiver.LastExpense, receiver.LastNet);
        }

        public List<(string CategoryId, string CategoryName, EntryType Type, int Total)> GroupByCategory(
            DateTime from, DateTime to, bool timed, string scenario)
        {
            var ops = _mgr.GetAllOperations();
            var receiver = new AnalyticsReceiver(ops);
            Command baseCmd = new GroupByCategoryCommand(receiver, from, to);

            if (timed && _store != null)
                _invoker.SetTimedLogged(baseCmd, _store, scenario);
            else
                _invoker.SetCommand(baseCmd);

            _invoker.Run();
            return receiver.LastTotals ?? new List<(string, string, EntryType, int)>();
        }

        public List<string> GetLogs() => _store != null ? _store.GetAll() : new List<string>();
        public void ClearLogs() => _store?.Clear();
    }
}
