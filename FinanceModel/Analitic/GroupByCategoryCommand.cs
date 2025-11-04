using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceModel.Analitic
{
    public class GroupByCategoryCommand : Command
    {
        private readonly AnalyticsReceiver _receiver;
        private readonly DateTime _from;
        private readonly DateTime _to;
        public GroupByCategoryCommand(AnalyticsReceiver receiver, DateTime from, DateTime to)
        { 
            _receiver = receiver; 
            _from = from; 
            _to = to; 
        }
        public override void Execute()
        {
            _receiver.GroupByCategory(_from, _to);
        }
        public override void Undo()
        { 
            _receiver.Clear(); 
        }
    }
}
