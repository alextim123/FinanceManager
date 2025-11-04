using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceModel.Export.Intefaces
{

    public interface IVisitable
    {
        public void Accept(IExportVisitor visitor);
    }
}
