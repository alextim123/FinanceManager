using FinanceModel.domain.entities;
using FinanceModel.Domain.Fabric.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FinanceModel.domain.Fabric
{
    public sealed class BankAccountFactory : IBankAccountFactory
    {
        public BankAccount Create(string id, string name, int balance)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ValidationException("ID счёта пуст.");
            if (id.Length > 200)
                throw new ValidationException("ID счёта слишком длинный (макс. 200).");

            if (string.IsNullOrWhiteSpace(name))
                throw new ValidationException("Название счёта пусто.");
            if (name.Length > 200)
                throw new ValidationException("Название счёта слишком длинное (макс. 200).");

            if (balance < 0)
                throw new ValidationException("Баланс не может быть отрицательным.");

            return new BankAccount(name, id, balance);
        }
    }
}
