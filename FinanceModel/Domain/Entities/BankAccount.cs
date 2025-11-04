using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceModel.domain.entities
{
    public class BankAccount
    {
        public string ID;
        public string Name;
        public int Balance;
        public BankAccount(string name, string id, int balance = 0)
        {
            ID = id;
            Name = name;
            Balance = balance;
        }
    }
}
