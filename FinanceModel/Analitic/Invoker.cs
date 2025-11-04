using FinanceModel.Decorators;
using FinanceModel.ErrorData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceModel.Analitic
{
    public class Invoker
    {
        private Command _command;

        public void SetCommand(Command c) => _command = c;
        public void SetTimedLogged(Command command, IErrorData store, string scenarioName)
        {
            var decorator = new TimedLoggingCommand(store, scenarioName);
            decorator.SetCommand(command);
            _command = decorator;
        }
        public void Run()
        {
            _command?.Execute();
        }
        public void Cancel()
        {
            _command?.Undo();
        }
    }
}
