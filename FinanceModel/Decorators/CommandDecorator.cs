using FinanceModel.Analitic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceModel.Decorators
{
    public abstract class CommandDecorator : Command
    {
        protected Command inner;
        public void SetCommand(Command command)
        {
            inner = command;
        }
        public override void Execute() { inner?.Execute(); }
        public override void Undo() { inner?.Undo(); }
    }
}
