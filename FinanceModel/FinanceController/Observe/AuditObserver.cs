using FinanceModel.domain.entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceModel.FinanceController.Observe
{
    public sealed class AuditObserver
    {
        public void OnAdded(Operation op)
            => Console.WriteLine($"[AUDIT] добавлено  op={op.ID} acc={op.Account.ID} cat={op.Category.ID} ammount={op.Amount}");

        public void OnRemoved(Operation op)
            => Console.WriteLine($"[AUDIT] удалено op={op.ID} acc={op.Account.ID} cat={op.Category.ID} ammount={op.Amount}");

        public void OnReplaced(Operation oldOp, Operation newOp)
            => Console.WriteLine($"[AUDIT] заменена op={oldOp.ID} -> op={newOp.ID} amt {oldOp.Amount}->{newOp.Amount}");
    }
}
