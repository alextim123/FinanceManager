using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceModel.ErrorData
{
    public sealed class MemoryErrorData: IErrorData
    {
        private readonly List<string> _logs = new();

        public void Add(string line) => _logs.Add(line);
        public List<string> GetAll() => _logs;
        public void Clear() => _logs.Clear();
    }
}
