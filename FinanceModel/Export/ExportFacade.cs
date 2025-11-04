using FinanceModel.ConsoleCreator;
using FinanceModel.domain.entities;
using FinanceModel.Export.Intefaces;
using System.Collections.Generic;

namespace FinanceModel.Export
{
    public sealed class ExportFacade
    {
        private readonly FinanceManager _mgr;
        public ExportFacade(FinanceManager mgr) { _mgr = mgr; }

        public void ExportAll(IExportVisitor visitor, string outputPath)
        {
            List<BankAccount> accounts = _mgr.GetAllBankAccounts();
            List<Category> categories = _mgr.GetAllCategories();
            List<Operation> operations = _mgr.GetAllOperations();

            visitor.Visit(accounts);
            visitor.Visit(categories);
            visitor.Visit(operations);

            visitor.Save(outputPath);
        }
    }
}
