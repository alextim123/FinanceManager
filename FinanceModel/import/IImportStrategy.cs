using FinanceModel.domain.entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceModel.import
{
    public interface IImportStrategy
    {
        public void SetFilePath(string path);
        public List<BankAccount> ParseAccounts();
        public List<Category> ParseCategories();
        public List<Operation> ParseOperations();
    }
}
