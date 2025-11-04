using FinanceModel.domain.entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceModel.Analitic
{
    public class AnalyticsReceiver
    {
        private readonly List<Operation> _ops;

        public int LastIncome { get; private set; }
        public int LastExpense { get; private set; }
        public int LastNet { get; private set; }
        public List<(string CategoryId, string CategoryName, EntryType Type, int Total)> LastTotals { get; private set; }

        public AnalyticsReceiver(List<Operation> ops)
        {
            _ops = ops;
        }

        public void CalcPeriodBalance(DateTime from, DateTime to)
        {
            var q = _ops.Where(o => o.Date >= from && o.Date <= to);
            LastIncome = q.Where(o => o.Type == EntryType.Income).Sum(o => o.Amount);
            LastExpense = q.Where(o => o.Type == EntryType.Expense).Sum(o => o.Amount);
            LastNet = LastIncome - LastExpense;
            LastTotals = null;
        }

        public void GroupByCategory(DateTime from, DateTime to)
        {
            var q = _ops.Where(o => o.Date >= from && o.Date <= to);
            LastTotals = q
                .GroupBy(o => o.Category)
                .Select(g => (g.Key.ID, g.Key.Name, g.Key.Type, g.Sum(x => x.Amount)))
                .OrderByDescending(x => x.Item4)
                .Select(t => (t.ID, t.Name, t.Type, t.Item4))
                .ToList();
            LastIncome = LastExpense = LastNet = 0;
        }

        public void Clear()
        {
            LastIncome = LastExpense = LastNet = 0;
            LastTotals = null;
        }
    }
}
