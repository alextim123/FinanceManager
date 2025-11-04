using FinanceModel.domain.entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceModel.Export.Intefaces
{
    public interface IExportVisitor
    {
        public void Visit(List<BankAccount> accounts);
        public void Visit(List<Category> categories);
        public void Visit(List<Operation> operations);
        public void Save(string path);
    }
}
