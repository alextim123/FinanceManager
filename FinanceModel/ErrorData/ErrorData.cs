using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceModel.ErrorData
{
    public interface IErrorData
    {
        void Add(string line);
        List<string> GetAll();
        public void Clear();
    }
}
