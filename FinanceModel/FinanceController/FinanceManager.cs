using FinanceModel.domain.entities;
using FinanceModel.repository.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceModel.ConsoleCreator
{
    public class FinanceManager
    {
        public event Action<Operation>? OperationAdded;
        public event Action<Operation>? OperationRemoved;
        public event Action<Operation, Operation>? OperationReplaced;
        IRepoOperation _repoOper;
        IRepoCategory _repoCat;
        IRepoBankAccount _repoAcc;
        public FinanceManager(IRepoOperation repoOper, IRepoCategory repoCat, IRepoBankAccount repoAcc) 
        {
            _repoAcc = repoAcc;
            _repoOper = repoOper;
            _repoCat = repoCat;
        }
        private static int Sign(EntryType t) => t == EntryType.Income ? +1 : -1;

        //Добавление сущностей с проверкой Id 
        public (bool ok, string? error) TryAddBankAccount(BankAccount acc)
        {
            if (_repoAcc.GetAllBankAccount().Any(a => a.ID == acc.ID))
                return (false, $"Счёт с ID '{acc.ID}' уже существует.");
            _repoAcc.Save(acc);
            return (true, null);
        }

        public (bool ok, string? error) TryAddCategory(Category cat)
        {
            if (_repoCat.GetAllCategory().Any(c => c.ID == cat.ID))
                return (false, $"Категория с ID '{cat.ID}' уже существует.");
            _repoCat.Save(cat);
            return (true, null);
        }

        
        //Удаление сущностей с проверкой Id
        public (bool ok, string? error) TryRemoveBankAccountById(string id)
        {
            var acc = _repoAcc.GetAllBankAccount().FirstOrDefault(a => a.ID == id);
            if (acc == null) return (false, $"Счёт с ID '{id}' не найден.");
            _repoAcc.Delete(acc);
            return (true, null);
        }

        public (bool ok, string? error) TryRemoveCategoryById(string id)
        {
            var cat = _repoCat.GetAllCategory().FirstOrDefault(c => c.ID == id);
            if (cat == null) return (false, $"Категория с ID '{id}' не найдена.");
            _repoCat.Delete(cat);
            return (true, null);
        }

       
        //Очистка репозиториев
        public void RemoveAllBankAccounts() => _repoAcc.DeleteAll();
        public void RemoveAllCategories() => _repoCat.DeleteAll();
        public void RemoveAllOperations() => _repoOper.DeleteAll();
        //Замена сущностей
        public (bool ok, string? error) TryReplaceBankAccount(BankAccount acc)
        {
            if (!_repoAcc.GetAllBankAccount().Any(a => a.ID == acc.ID))
                return (false, $"Счёт с ID '{acc.ID}' не найден.");
            _repoAcc.Update(acc);
            return (true, null);
        }

        public (bool ok, string? error) TryReplaceCategory(Category cat)
        {
            if (!_repoCat.GetAllCategory().Any(c => c.ID == cat.ID))
                return (false, $"Категория с ID '{cat.ID}' не найдена.");
            _repoCat.Update(cat);
            return (true, null);
        }

        //Получение data из репозиториев
        public List<BankAccount> GetAllBankAccounts() => _repoAcc.GetAllBankAccount();
        public List<Category> GetAllCategories() => _repoCat.GetAllCategory();
        public List<Operation> GetAllOperations() => _repoOper.GetAllOperation();

        private void AdjustBalance(string accountId, int delta)
        {
            var acc = _repoAcc.GetAllBankAccount().FirstOrDefault(a => a.ID == accountId);
            if (acc is null) return; 
            _repoAcc.Update(new BankAccount(acc.Name, acc.ID, acc.Balance + delta));
        }

        //Действия с операциями
       
        public (bool ok, string? error) TryAddOperation(Operation op)
        {
            if (_repoOper.GetAllOperation().Any(o => o.ID == op.ID))
                return (false, $"Операция с ID '{op.ID}' уже существует.");

            _repoOper.Save(op);
            AdjustBalance(op.Account.ID, Sign(op.Type) * op.Amount);

            OperationAdded?.Invoke(op);
            return (true, null);
        }

    
        public (bool ok, string? error) TryRemoveOperationById(string id)
        {
            var op = _repoOper.GetAllOperation().FirstOrDefault(o => o.ID == id);
            if (op == null) return (false, $"Операция с ID '{id}' не найдена.");

            _repoOper.Delete(op);
            AdjustBalance(op.Account.ID, -Sign(op.Type) * op.Amount);

            OperationRemoved?.Invoke(op);
            return (true, null);
        }

      
        public (bool ok, string? error) TryReplaceOperation(Operation op)
        {
            var old = _repoOper.GetAllOperation().FirstOrDefault(o => o.ID == op.ID);
            if (old == null) return (false, $"Операция с ID '{op.ID}' не найдена.");

            _repoOper.Update(op);
            var delta = (Sign(op.Type) * op.Amount) - (Sign(old.Type) * old.Amount);
            AdjustBalance(op.Account.ID, delta);

            OperationReplaced?.Invoke(old, op);
            return (true, null);
        }
    }
}
