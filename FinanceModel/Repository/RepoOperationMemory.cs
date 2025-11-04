using FinanceModel.domain.entities;
using FinanceModel.repository.interfaces;
using System.Collections.Generic;
using System.Linq;

namespace FinanceModel.repository
{
    public class RepoOperationMemory: IRepoOperation
    {
        private Dictionary<string, Operation> _operationData = new();

        public void Save(Operation oper)
        {
            _operationData.Add(oper.ID, oper);
        }

        public void Delete(Operation oper)
        {
            _operationData.Remove(oper.ID);
        }

        public void Update(Operation oper)
        {
            _operationData[oper.ID] = oper;
        }

        public void DeleteAll()
        {
            _operationData.Clear();
        }

        public void UpdateAll(List<Operation> opers)
        {
            var result = new Dictionary<string, Operation>();
            for (int i = 0; i < opers.Count; i++)
            {
                result.Add(opers[i].ID, opers[i]);
            }
            _operationData = result;
        }

        public List<Operation> GetAllOperation()
        {
            return _operationData.Values.ToList();
        }
    }
}
