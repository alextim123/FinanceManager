using FinanceModel.domain.entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceModel.import
{
    public abstract class ImportStrategy: IImportStrategy
    {
        public abstract void SetFilePath(string path);
        public abstract List<BankAccount> ParseAccounts();
        public abstract List<Category> ParseCategories();
        public abstract List<Operation> ParseOperations();
        public (List<Operation>, List<Category>, List<BankAccount>) TemplateParse(string path)
        {
            SetFilePath(path);
            List<BankAccount> accounts = ParseAccounts();
            List<Category> categories = ParseCategories();
            List<Operation> operations = ParseOperations();
            return (operations, categories, accounts);
        }
    }
}
