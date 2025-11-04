using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceModel.domain.entities
{
    public class Operation
    {
        public string ID;
        public EntryType Type;
        public BankAccount Account;  
        public Category Category;   
        public int Amount;
        public DateTime Date;
        public string? Description;
        public Operation(string id, EntryType type, BankAccount account, Category category,int amount, DateTime date, string? description = null)
        {
            ID = id;
            Type = type;
            Account = account;
            Category = category;
            Amount = amount;
            Date = date;
            Description = description;
        }
    }
}
