using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FinanceModel.domain.entities;
using System.Collections.Generic;
using System.Linq;
using FinanceModel.repository.interfaces;

namespace FinanceModel.repository
{
    public class RepoBankAccountMemory: IRepoBankAccount
    {
        private Dictionary<string, BankAccount> _accountData = new();

        public void Save(BankAccount account)
        {
            _accountData.Add(account.ID, account);
        }

        public void Delete(BankAccount account)
        {
            _accountData.Remove(account.ID);
        }

        public void Update(BankAccount account)
        {
            _accountData[account.ID] = account;
        }

        public void DeleteAll()
        {
            _accountData.Clear();
        }

        public void UpdateAll(List<BankAccount> accounts)
        {
            var result = new Dictionary<string, BankAccount>();
            for (int i = 0; i < accounts.Count; i++)
            {
                result.Add(accounts[i].ID, accounts[i]);
            }
            _accountData = result;
        }

        public List<BankAccount> GetAllBankAccount()
        {
            return _accountData.Values.ToList();
        }
    }
}

