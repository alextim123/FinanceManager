using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceModel.Analitic
{
    public class CalcPeriodBalanceCommand : Command
    {
        private readonly AnalyticsReceiver _receiver;
        private readonly DateTime _from;
        private readonly DateTime _to;
        public CalcPeriodBalanceCommand(AnalyticsReceiver receiver, DateTime from, DateTime to) 
        { 
            _receiver = receiver; 
            _from = from; 
            _to = to; 
        }
        public override void Execute()
        {
            _receiver.CalcPeriodBalance(_from, _to);
        }

        public override void Undo() 
        {
            _receiver.Clear();
        }
    }
}
