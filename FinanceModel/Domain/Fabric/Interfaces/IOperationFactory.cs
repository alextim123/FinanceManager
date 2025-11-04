using FinanceModel.domain.entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceModel.Domain.Fabric.Interfaces
{
    public interface IOperationFactory
    {
        Operation Create(
            string id,
            EntryType type,
            BankAccount account,
            Category category,
            int amount,
            string dateDym,
            string? description);
    }
}
