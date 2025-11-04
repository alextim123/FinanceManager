using FinanceModel.domain.entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceModel.Domain.Fabric.Interfaces
{
    public interface IBankAccountFactory
    {
        BankAccount Create(string id, string name, int balance);
    }
}
