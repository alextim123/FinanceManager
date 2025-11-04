using FinanceModel.domain.entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceModel.repository.interfaces
{
    public interface IRepoBankAccount
    {
        public void Save(BankAccount account);
        public void Delete(BankAccount account);
        public void Update(BankAccount account);
        public void DeleteAll();
        public void UpdateAll(List<BankAccount> accounts);
        public List<BankAccount> GetAllBankAccount();
    }
}
